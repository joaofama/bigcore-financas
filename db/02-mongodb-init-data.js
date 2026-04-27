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

print("2 - Carga finalizada com sucesso!");