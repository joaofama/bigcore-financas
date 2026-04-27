db = db.getSiblingDB('FinancasDb');

print("1/2 - Iniciando carga total de categorias (Modelo Plano) e usuário de teste...");

// Usando a função UUID para salvar como BinData (Guid nativo)
var usuarioId = UUID("11111111-1111-1111-1111-111111111111");

// Inserção do Usuário
if (!db.Usuarios.findOne({ _id: usuarioId })) {
    db.Usuarios.insertOne({
        _id: usuarioId,
        nome: "Usuário de Teste",
        email: "teste@teste.com",
        senha: "teste@123",        
        saldoInicial: NumberDecimal("0.00"),
        ativo: true,
        dataCadastro: new ISODate(),
        dataAlteracao: null 
    });
}

// Limpa categorias existentes
db.Categorias.deleteMany({ usuarioId: usuarioId });

var categorias = [
    // --- RECEITAS (tipo: "R") ---
    {
        _id: UUID("a1111111-0000-0000-0000-000000000001"),
        usuarioId: usuarioId,
        nome: "Salário",
        tipo: "R",
        icone: "mdi-cash-main",
        categoriaPaiId: null,
        ativo: true
    },
    {
        _id: UUID("a1111111-1000-0000-0000-000000000001"),
        usuarioId: usuarioId,
        nome: "Adiantamento",
        tipo: "R",
        icone: "mdi-cash-fast",
        categoriaPaiId: UUID("a1111111-0000-0000-0000-000000000001"),
        ativo: true
    },
    {
        _id: UUID("a1111111-2000-0000-0000-000000000001"),
        usuarioId: usuarioId,
        nome: "Saldo Mensal",
        tipo: "R",
        icone: "mdi-cash-check",
        categoriaPaiId: UUID("a1111111-0000-0000-0000-000000000001"),
        ativo: true
    },

    // --- INVESTIMENTOS ---
    {
        _id: UUID("a1111111-0000-0000-0000-000000000002"),
        usuarioId: usuarioId,
        nome: "Investimentos",
        tipo: "R",
        icone: "mdi-chart-line",
        categoriaPaiId: null,
        ativo: true
    },
    {
        _id: UUID("a1111111-1000-0000-0000-000000000002"),
        usuarioId: usuarioId,
        nome: "Dividendos",
        tipo: "R",
        icone: "mdi-bank-transfer-in",
        categoriaPaiId: UUID("a1111111-0000-0000-0000-000000000002"),
        ativo: true
    },

    // --- DESPESAS (tipo: "D"): MORADIA ---
    {
        _id: UUID("b2222222-0000-0000-0000-000000000001"),
        usuarioId: usuarioId,
        nome: "Moradia",
        tipo: "D",
        icone: "mdi-home",
        categoriaPaiId: null,
        ativo: true
    },
    {
        _id: UUID("b2222222-1000-0000-0000-000000000001"),
        usuarioId: usuarioId,
        nome: "Aluguel/Condomínio",
        tipo: "D",
        icone: "mdi-home-city",
        categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000001"),
        ativo: true
    },

    // --- DESPESAS: ALIMENTAÇÃO ---
    {
        _id: UUID("b2222222-0000-0000-0000-000000000002"),
        usuarioId: usuarioId,
        nome: "Alimentação",
        tipo: "D",
        icone: "mdi-food",
        categoriaPaiId: null,
        ativo: true
    },
    {
        _id: UUID("b2222222-1000-0000-0000-000000000002"),
        usuarioId: usuarioId,
        nome: "Supermercado",
        tipo: "D",
        icone: "mdi-cart",
        categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000002"),
        ativo: true
    },
    {
        _id: UUID("b2222222-2000-0000-0000-000000000002"),
        usuarioId: usuarioId,
        nome: "Restaurantes/Ifood",
        tipo: "D",
        icone: "mdi-silverware-fork-knife",
        categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000002"),
        ativo: true
    },

    // --- DESPESAS: TRANSPORTE ---
    {
        _id: UUID("b2222222-0000-0000-0000-000000000003"),
        usuarioId: usuarioId,
        nome: "Transporte",
        tipo: "D",
        icone: "mdi-car",
        categoriaPaiId: null,
        ativo: true
    },
    {
        _id: UUID("b2222222-1000-0000-0000-000000000003"),
        usuarioId: usuarioId,
        nome: "Combustível",
        tipo: "D",
        icone: "mdi-gas-station",
        categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000003"),
        ativo: true
    },

    // --- DESPESAS: LAZER ---
    {
        _id: UUID("b2222222-0000-0000-0000-000000000005"),
        usuarioId: usuarioId,
        nome: "Lazer",
        tipo: "D",
        icone: "mdi-movie",
        categoriaPaiId: null,
        ativo: true
    },
    {
        _id: UUID("b2222222-2000-0000-0000-000000000005"),
        usuarioId: usuarioId,
        nome: "Streaming (Netflix/Spotify)",
        tipo: "D",
        icone: "mdi-play-circle",
        categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000005"),
        ativo: true
    }
];

db.Categorias.insertMany(categorias);

// --- CARGA DE TRANSAÇÕES (JAN/2026 A MAIO/2026) ---
print("3/3 - Gerando massa de dados de transações...");

db.Transacoes.deleteMany({ usuarioId: usuarioId });

var transacoesFake = [];

// Função auxiliar para criar a data
function criarData(ano, mes, dia) {
    return new ISODate(ano + "-" + (mes < 10 ? "0" + mes : mes) + "-" + (dia < 10 ? "0" + dia : dia) + "T12:00:00Z");
}

// Loop pelos meses: 1 (Jan) até 5 (Maio)
for (var m = 1; m <= 5; m++) {
    var ano = 2026;

    // --- RECEITAS FIXAS ---
    // Adiantamento (Dia 15)
    transacoesFake.push({
        _id: UUID(),
        usuarioId: usuarioId,
        descricao: "Adiantamento Quinzenal",
        valor: NumberDecimal("4500.00"),
        data: criarData(ano, m, 15),
        tipo: "R",
        categoriaId: UUID("a1111111-1000-0000-0000-000000000001"),
        categoriaNome: "Adiantamento",
        categoriaIcone: "mdi-cash-fast",
        categoriaPaiId: UUID("a1111111-0000-0000-0000-000000000001"),
        categoriaPaiNome: "Salário",
        categoriaPaiIcone: "mdi-cash-main",
        dataCriacao: new ISODate()
    });

    // Saldo Mensal (Dia 30)
    transacoesFake.push({
        _id: UUID(),
        usuarioId: usuarioId,
        descricao: "Pagamento Mensal",
        valor: NumberDecimal("7250.00"),
        data: criarData(ano, m, 28),
        tipo: "R",
        categoriaId: UUID("a1111111-2000-0000-0000-000000000001"),
        categoriaNome: "Saldo Mensal",
        categoriaIcone: "mdi-cash-check",
        categoriaPaiId: UUID("a1111111-0000-0000-0000-000000000001"),
        categoriaPaiNome: "Salário",
        categoriaPaiIcone: "mdi-cash-main",
        dataCriacao: new ISODate()
    });

    // --- DESPESAS FIXAS ---
    // Aluguel/Condomínio (Dia 5)
    transacoesFake.push({
        _id: UUID(),
        usuarioId: usuarioId,
        descricao: "Aluguel e Condomínio",
        valor: NumberDecimal("3155.00"),
        data: criarData(ano, m, 5),
        tipo: "D",
        categoriaId: UUID("b2222222-1000-0000-0000-000000000001"),
        categoriaNome: "Aluguel/Condomínio",
        categoriaIcone: "mdi-home-city",
        categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000001"),
        categoriaPaiNome: "Moradia",
        categoriaPaiIcone: "mdi-home",
        dataCriacao: new ISODate()
    });

    // Streaming (Dia 10)
    transacoesFake.push({
        _id: UUID(),
        usuarioId: usuarioId,
        descricao: "Netflix/Spotify",
        valor: NumberDecimal("85.90"),
        data: criarData(ano, m, 10),
        tipo: "D",
        categoriaId: UUID("b2222222-2000-0000-0000-000000000005"),
        categoriaNome: "Streaming (Netflix/Spotify)",
        categoriaIcone: "mdi-play-circle",
        categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000005"),
        categoriaPaiNome: "Lazer",
        categoriaPaiIcone: "mdi-movie",
        dataCriacao: new ISODate()
    });

    // --- DESPESAS VARIÁVEIS (Simulando 3 compras de mercado e 2 ifoods por mês) ---
    var diasMercado = [2, 12, 22];
    diasMercado.forEach(function(dia) {
        transacoesFake.push({
            _id: UUID(),
            usuarioId: usuarioId,
            descricao: "Compras Supermercado",
            valor: NumberDecimal((Math.random() * (600 - 300) + 300).toFixed(2)),
            data: criarData(ano, m, dia),
            tipo: "D",
            categoriaId: UUID("b2222222-1000-0000-0000-000000000002"),
            categoriaNome: "Supermercado",
            categoriaIcone: "mdi-cart",
            categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000002"),
            categoriaPaiNome: "Alimentação",
            categoriaPaiIcone: "mdi-food",
            dataCriacao: new ISODate()
        });
    });

    var diasIfood = [7, 18, 25];
    diasIfood.forEach(function(dia) {
        transacoesFake.push({
            _id: UUID(),
            usuarioId: usuarioId,
            descricao: "Restaurante/Delivery",
            valor: NumberDecimal((Math.random() * (150 - 60) + 60).toFixed(2)),
            data: criarData(ano, m, dia),
            tipo: "D",
            categoriaId: UUID("b2222222-2000-0000-0000-000000000002"),
            categoriaNome: "Restaurantes/Ifood",
            categoriaIcone: "mdi-silverware-fork-knife",
            categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000002"),
            categoriaPaiNome: "Alimentação",
            categoriaPaiIcone: "mdi-food",
            dataCriacao: new ISODate()
        });
    });

    // Combustível (2x por mês)
    [8, 24].forEach(function(dia) {
        transacoesFake.push({
            _id: UUID(),
            usuarioId: usuarioId,
            descricao: "Posto de Combustível",
            valor: NumberDecimal("220.00"),
            data: criarData(ano, m, dia),
            tipo: "D",
            categoriaId: UUID("b2222222-1000-0000-0000-000000000003"),
            categoriaNome: "Combustível",
            categoriaIcone: "mdi-gas-station",
            categoriaPaiId: UUID("b2222222-0000-0000-0000-000000000003"),
            categoriaPaiNome: "Transporte",
            categoriaPaiIcone: "mdi-car",
            dataCriacao: new ISODate()
        });
    });
}

db.Transacoes.insertMany(transacoesFake);

print("Finalizado: " + transacoesFake.length + " transações inseridas.");