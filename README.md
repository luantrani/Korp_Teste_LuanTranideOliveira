# Korp Teste LuanTrani de Oliveira

Projeto de exemplo para um sistema de notas fiscais com microsserviços em C#.

## Objetivo

- Implementar cadastro de produtos e notas fiscais utilizando Angular no frontend e C# no backend.
- Criar dois microsserviços: Estoque e Faturamento.
- Uso de banco de dados local SQLite para persistência.
- Comunicação entre microsserviços via HTTP/REST.

## Arquitetura iniciada

- `backend/src/EstoqueService`: serviço REST para controle de produtos e saldos.
- `backend/src/FaturamentoService`: serviço REST para gestão de notas fiscais.
- `frontend`: scaffold inicial da aplicação Angular com componente raiz e configuração de build.

## Próximo passo

1. Confirmar esta estrutura inicial.
2. Adicionar o frontend Angular.
3. Implementar chamadas entre microsserviços (Faturamento → Estoque).
4. Completar a interface de cadastro e impressão de notas.


