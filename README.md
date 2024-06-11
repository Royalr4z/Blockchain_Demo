<h1>Blockchain_Demo</h1>

## Tecnologias Utilizadas

- **Linguagem:** C#/C++
- **Framework:** .NET

## Instalação e Configuração (Fedora Linux)

1. Certifique-se de ter o .NET e o Compilador C++ instalado em sua máquina.
   - Você pode instalar o .NET seguindo as instruções fornecidas em [dotnet.microsoft.com](https://dotnet.microsoft.com/download)
   - Para instalar o compilador C++, você pode usar o seguinte comando:
     ```
     sudo dnf install gcc-c++
     ```

2. Instale o curl:
     ```
     sudo dnf install libcurl-devel
     ```

3. Clone este repositório:
     ```
     git clone https://github.com/Royalr4z/Blockchain_Demo.git
     ```

4. Compile e execute o servidor de recebimento da Blockchain via TCP/IP:
     ```
     g++ -o server server.cpp -lcurl && ./server
     ```

5. Execute o seguinte comando para iniciar o servidor de Atualização e Envio:
     ```
     dotnet run
     ```

## Rotas/APIs Disponíveis

A seguir estão as principais rotas e APIs fornecidas pelo backend.

### 1. Blockchain

---
- **Descrição:** API de Visualização da Blockchain
- **Método HTTP:** [GET]
- **Exemplo de Requisição:**
  ```bash
  curl -X GET http://localhost:7000/Blockchain

---
- **Descrição:** API de Criação de um Novo Bloco
- **Método HTTP:** [POST]
- **Exemplo de Requisição:**
  ```bash
  curl -X POST http://localhost:7000/Blockchain

### 2. Mempool

---
- **Descrição:** API de Visualização da Mempool
- **Método HTTP:** [GET]
- **Exemplo de Requisição:**
  ```bash
  curl -X GET http://localhost:7000/mempool

---
- **Descrição:** API de Inserção de uma nova Transação na Mempool
- **Método HTTP:** [POST]
- **Exemplo de Requisição:**
  ```bash
  curl -X POST http://localhost:7000/mempool -d '{
      "transactions": [
          { "from": "de", "towards": "para", "value": 0.2, "rate": 0.01 },
          { "from": "de", "towards": "para", "value": 0.7, "rate": 0.01 },
          { "from": "de", "towards": "para", "value": 0.01, "rate": 0.001 },
          { "from": "de", "towards": "para", "value": 0.5, "rate": 0.01 },
          { "from": "de", "towards": "para", "value": 0.1, "rate": 0.01 },
          { "from": "de", "towards": "para", "value": 0.4, "rate": 0.01 }
      ]
  }' -H "Content-Type: application/json"

### 3. user

---
- **Descrição:** API de Visualização da Private Key, Public Key e dos Address
- **Método HTTP:** [GET]
- **Exemplo de Requisição:**
  ```bash
  curl -X GET http://localhost:7000/user

### 4. P2P

---
- **Descrição:** API de Implemetação da Rede P2P
- **Método HTTP:** [POST]
- **Exemplo de Requisição:**
  ```bash
  curl -X POST http://localhost:7000/P2P -d '[
    {
      "index": 0,
      "uBits": 5,
      "nonce": 1124781,
      "timestamp": "11/06/2024 12:00:58",
      "merkleRoot": "0000000000000000000000000000000000000000000000000000000000000000",
      "hash": "00000753b7751da679acea0856a5187914b6b1d2ebd9e6c245e6d5e0ef69c204",
      "previous_hash": "0",
      "txnCounter": 0,
      "transactions": []
    }
  ]' -H "Content-Type: application/json"

# Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues relatando problemas, sugestões ou novos recursos que gostaria de ver. Se deseja contribuir com código, por favor, envie um pull request.
