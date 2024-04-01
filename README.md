<h1>Blockchain_Demo</h1>

## Tecnologias Utilizadas

- **Linguagem:** C#
- **Framework:** .NET

## Instalação e Configuração

1. Certifique-se de ter o .NET instalado em sua máquina.
2. Clone este repositório.
3. Execute `dotnet run` para iniciar o servidor.

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
  curl -X POST http://localhost:7000/Blockchain -d '{
      "transactions": [
          { "from": "de", "towards": "para", "value": 0.2, "rate": 0.01 },
          { "from": "de", "towards": "para", "value": 0.7, "rate": 0.01 },
          { "from": "de", "towards": "para", "value": 0.01, "rate": 0.001 },
          { "from": "de", "towards": "para", "value": 0.5, "rate": 0.01 },
          { "from": "de", "towards": "para", "value": 0.1, "rate": 0.01 },
          { "from": "de", "towards": "para", "value": 0.4, "rate": 0.01 }
      ]
  }' -H "Content-Type: application/json"


# Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues relatando problemas, sugestões ou novos recursos que gostaria de ver. Se deseja contribuir com código, por favor, envie um pull request.
