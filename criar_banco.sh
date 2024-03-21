#!/bin/bash

# Variáveis de configuração
source ./.env

NOME_BANCO=$DATABASE_NAME
USUARIO=$DATABASE_USER
SENHA=$DATABASE_PASSWORD

# Exporte a senha como variável de ambiente para uso seguro
export PGPASSWORD=$SENHA

# Comando SQL para criar o banco de dados
CREATE_USERS="""
    CREATE TABLE users (
        id SERIAL PRIMARY KEY,
        name VARCHAR NOT NULL,
        email VARCHAR UNIQUE NOT NULL,
        password VARCHAR NOT NULL,
        admin BOOLEAN DEFAULT false NOT NULL
    );
"""

CREATE_CATEGORY="""
    CREATE TABLE category (
        id SERIAL PRIMARY KEY,
        name VARCHAR UNIQUE NOT NULL,
        subtitle VARCHAR NOT NULL
    );
"""

CREATE_BLOGS="""
    CREATE TABLE blogs (
        id SERIAL PRIMARY KEY,
        date VARCHAR NOT NULL,
        title VARCHAR NOT NULL,
        subtitle VARCHAR NOT NULL,
        imageurl VARCHAR(2083),
        content TEXT NOT NULL,
        userid INTEGER REFERENCES users(id) NOT NULL,
        categoryid INTEGER REFERENCES category(id) NOT NULL
    );
"""

CREATE_FREE_QUOTE="""
    CREATE TABLE free_quote (
        id SERIAL PRIMARY KEY,
        date VARCHAR NOT NULL,
        name VARCHAR NOT NULL,
        email VARCHAR NOT NULL,
        service VARCHAR NOT NULL,
        message VARCHAR(1000) NOT NULL
    );
"""

CREATE_MESSAGE="""
    CREATE TABLE message (
        id SERIAL PRIMARY KEY,
        date VARCHAR NOT NULL,
        name VARCHAR NOT NULL,
        email VARCHAR NOT NULL,
        subject VARCHAR NOT NULL,
        content VARCHAR(5000) NOT NULL
    );
"""

# Conectar e executar o comando SQL
psql -U $USUARIO -h localhost -d $NOME_BANCO -c "$CREATE_USERS"
psql -U $USUARIO -h localhost -d $NOME_BANCO -c "$CREATE_CATEGORY"
psql -U $USUARIO -h localhost -d $NOME_BANCO -c "$CREATE_BLOGS"
psql -U $USUARIO -h localhost -d $NOME_BANCO -c "$CREATE_FREE_QUOTE"
psql -U $USUARIO -h localhost -d $NOME_BANCO -c "$CREATE_MESSAGE"

# Limpe a variável de ambiente PGPASSWORD após o uso
unset PGPASSWORD

echo "Tabelas do Banco de dados $NOME_BANCO criado com sucesso."
