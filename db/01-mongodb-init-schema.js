db = db.getSiblingDB('FinancasDb');

print("1 Criando coleções e índices...");

db.createCollection('Usuarios');
db.createCollection('Categorias');
db.createCollection('Transacoes');

// Índices para garantir performance e integridade
db.Usuarios.createIndex({ "email": 1 }, { unique: true });
db.Transacoes.createIndex({ "usuarioId": 1, "data": -1 });
db.Categorias.createIndex({ "usuarioId": 1 });

print("2 - Estrutura pronta.");