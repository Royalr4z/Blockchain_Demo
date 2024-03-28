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

### 1. Block

- **Descrição:** API de Visualização da Blockchain
- **Método HTTP:** [GET]
- **Exemplo de Requisição:**
  ```bash
  curl -X GET http://localhost:7000/Block

### 2. mineblock

- **Descrição:** API de Criação de um Novo Bloco
- **Método HTTP:** [POST]
- **Exemplo de Requisição:**
  ```bash
  curl -X POST http://localhost:7000/mineblock -d '{
      "transactions": [
          { "id_transaction": "id_index-1", "from": "de", "towards": "para", "value": 0.2, "rate": 0.01 },
          { "id_transaction": "id_index-2", "from": "de", "towards": "para", "value": 0.7, "rate": 0.01 },
          { "id_transaction": "id_index-3", "from": "de", "towards": "para", "value": 0.01, "rate": 0.001 },
          { "id_transaction": "id_index-4", "from": "de", "towards": "para", "value": 0.5, "rate": 0.01 },
          { "id_transaction": "id_index-5", "from": "de", "towards": "para", "value": 0.1, "rate": 0.01 },
          { "id_transaction": "id_index-6", "from": "de", "towards": "para", "value": 0.4, "rate": 0.01 }
      ]
  }' -H "Content-Type: application/json"

