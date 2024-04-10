#include <iostream>
#include <string>
#include <cstring>
#include <fstream>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <unistd.h>
#include <vector>
#include <curl/curl.h>


class MethodIp {
    public:
        std::vector<std::string> ips;

        void updateIps(std::string newIP) {

            std::string filepath = "IPS.txt";
            std::ifstream file(filepath, std::ios_base::app);

            // verifica se o arquivo não existe e cria um novo se necessário
            if (!file.is_open()) {
                std::ofstream newFile(filepath);
                newFile.close();
            } else {
                file.close();
            }

            std::ofstream updateFile(filepath, std::ios_base::app);

            // Adicionando o ip no final do Arquivo
            if (updateFile.is_open()) {
                updateFile << newIP << std::endl;
                updateFile.close();
            }

            // Atualizando ips
            std::ifstream readFile(filepath);
            if (readFile.is_open()) {
                std::string ip;
                while (std::getline(readFile, ip)) {
                    ips.push_back(ip);
                }
                readFile.close();
            }
        }
};

void Requests(std::string hex) {
    curl_global_init(CURL_GLOBAL_ALL);
    CURL *curl = curl_easy_init();
    if (!curl) {
        std::cerr << "[-] Erro ao inicializar libcurl\n";
        return;
    }

    const char *url = "http://localhost:7000/P2P";

    std::string jsonData;
    for (size_t i = 0; i < hex.length(); i += 2) {
        std::string byteString = hex.substr(i, 2);
        char byte = static_cast<char>(std::stoi(byteString, nullptr, 16));
        jsonData.push_back(byte);
    }

    curl_easy_setopt(curl, CURLOPT_URL, url);
    curl_easy_setopt(curl, CURLOPT_POSTFIELDS, jsonData.c_str());
    curl_easy_setopt(curl, CURLOPT_POSTFIELDSIZE, jsonData.size());

    CURLcode res = curl_easy_perform(curl);
    if (res != CURLE_OK) {
        std::cerr << "[-] Erro no POST: " << curl_easy_strerror(res) << std::endl;
    } else {
        std::cout << "[+] POST enviado com sucesso!" << std::endl;
    }

    curl_easy_cleanup(curl);
    curl_global_cleanup();
}

std::string server() {
    int serverSocket = socket(AF_INET, SOCK_STREAM, 0);
    if (serverSocket == -1) {
        std::cerr << "[-] Erro ao criar socket\n";
        return "";
    }

    struct sockaddr_in serverAddr;
    serverAddr.sin_family = AF_INET;
    serverAddr.sin_addr.s_addr = INADDR_ANY;
    serverAddr.sin_port = htons(7001);

    if (bind(serverSocket, (struct sockaddr*)&serverAddr, sizeof(serverAddr)) == -1) {
        std::cerr << "[-] Erro ao vincular socket\n";
        close(serverSocket);
        return "";
    }

    if (listen(serverSocket, 5) == -1) {
        std::cerr << "[-] Erro ao escutar conexões\n";
        close(serverSocket);
        return "";
    }

    std::cout << "[+] Servidor esperando por conexões...\n";

    struct sockaddr_in clientAddr;
    socklen_t clientAddrSize = sizeof(clientAddr);
    int clientSocket = accept(serverSocket, (struct sockaddr*)&clientAddr, &clientAddrSize);
    if (clientSocket == -1) {
        std::cerr << "[-] Erro ao aceitar conexão\n";
        close(serverSocket);
        return "";
    }

    char ipStr[INET_ADDRSTRLEN];
    inet_ntop(AF_INET, &(clientAddr.sin_addr), ipStr, INET_ADDRSTRLEN);

    std::string clientIP(ipStr);

    MethodIp MethodIp;
    MethodIp.updateIps(clientIP);

    char buffer[1024];
    std::string receivedData;
    while (true) {
        ssize_t bytesReceived = recv(clientSocket, buffer, sizeof(buffer), 0);
        if (bytesReceived <= 0) {
            if (bytesReceived == 0) {
                std::cout << "[+] Conexão encerrada pelo cliente\n";
            } else {
                std::cerr << "[-] Erro ao receber dados do cliente\n";
            }
            break;
        }

        receivedData.append(buffer, bytesReceived);
    }

    close(clientSocket);
    close(serverSocket);
    
    return receivedData;
}

int main() {
    while (true) {
    
        std::cout << "Iniciando Servidor" << std::endl;
        std::cout << "[+] Executando na Porta 7001" << std::endl;


        std::string hex = server();

        if (!hex.empty()) {
            Requests(hex);
        }

        hex = "";
        system("clear");
    }

    return 0;
}

