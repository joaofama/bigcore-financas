db = db.getSiblingDB('FinancasDb');

print("1/2 - Iniciando carga total de categorias e usuário de teste...");

var usuarioId = "11111111-1111-1111-1111-111111111111";

// Inserção do Usuário
if (!db.Usuarios.findOne({ _id: usuarioId })) {
    db.Usuarios.insertOne({
        _id: usuarioId,
        nome: "Usuário de Teste",
        email: "teste@financas.com",
        senha: "Financas@123",        
        ativo: true,
        dataCriacao: new ISODate()
    });
}

// Limpa categorias existentes para evitar duplicidade no seed
db.Categorias.deleteMany({ usuarioId: usuarioId });

var categorias = [
    // --- RECEITAS ---
    {
        _id: "a1111111-0000-0000-0000-000000000001",
        usuarioId: usuarioId,
        nome: "Salário",
        icone: "mdi-cash-main",
        cor: "#2ECC71",
        ativo: true,
        subcategorias: [
            { id: "a1111111-1000-0000-0000-000000000001", nome: "Adiantamento", ativo: true },
            { id: "a1111111-2000-0000-0000-000000000001", nome: "Saldo Mensal", ativo: true },
            { id: "a1111111-3000-0000-0000-000000000001", nome: "Bônus/PLR", ativo: true }
        ]
    },
    {
        _id: "a1111111-0000-0000-0000-000000000002",
        usuarioId: usuarioId,
        nome: "Investimentos",
        icone: "mdi-chart-line",
        cor: "#27AE60",
        ativo: true,
        subcategorias: [
            { id: "a1111111-1000-0000-0000-000000000002", nome: "Dividendos", ativo: true },
            { id: "a1111111-2000-0000-0000-000000000002", nome: "Rendimentos CDB", ativo: true }
        ]
    },

    // --- DESPESAS: MORADIA ---
    {
        _id: "b2222222-0000-0000-0000-000000000001",
        usuarioId: usuarioId,
        nome: "Moradia",
        icone: "mdi-home",
        cor: "#3498DB",
        ativo: true,
        subcategorias: [
            { id: "b2222222-1000-0000-0000-000000000001", nome: "Aluguel/Condomínio", ativo: true },
            { id: "b2222222-2000-0000-0000-000000000001", nome: "Luz/Água/Gás", ativo: true },
            { id: "b2222222-3000-0000-0000-000000000001", nome: "Internet/TV", ativo: true },
            { id: "b2222222-4000-0000-0000-000000000001", nome: "Manutenção/Reforma", ativo: true }
        ]
    },

    // --- DESPESAS: ALIMENTAÇÃO ---
    {
        _id: "b2222222-0000-0000-0000-000000000002",
        usuarioId: usuarioId,
        nome: "Alimentação",
        icone: "mdi-food",
        cor: "#E67E22",
        ativo: true,
        subcategorias: [
            { id: "b2222222-1000-0000-0000-000000000002", nome: "Supermercado", ativo: true },
            { id: "b2222222-2000-0000-0000-000000000002", nome: "Restaurantes/Ifood", ativo: true },
            { id: "b2222222-3000-0000-0000-000000000002", nome: "Lanches/Padaria", ativo: true }
        ]
    },

    // --- DESPESAS: TRANSPORTE ---
    {
        _id: "b2222222-0000-0000-0000-000000000003",
        usuarioId: usuarioId,
        nome: "Transporte",
        icone: "mdi-car",
        cor: "#95A5A6",
        ativo: true,
        subcategorias: [
            { id: "b2222222-1000-0000-0000-000000000003", nome: "Combustível", ativo: true },
            { id: "b2222222-2000-0000-0000-000000000003", nome: "Uber/Transporte Público", ativo: true },
            { id: "b2222222-3000-0000-0000-000000000003", nome: "Seguro/IPVA", ativo: true },
            { id: "b2222222-4000-0000-0000-000000000003", nome: "Manutenção Veículo", ativo: true }
        ]
    },

    // --- DESPESAS: SAÚDE ---
    {
        _id: "b2222222-0000-0000-0000-000000000004",
        usuarioId: usuarioId,
        nome: "Saúde",
        icone: "mdi-heart-pulse",
        cor: "#E74C3C",
        ativo: true,
        subcategorias: [
            { id: "b2222222-1000-0000-0000-000000000004", nome: "Plano de Saúde", ativo: true },
            { id: "b2222222-2000-0000-0000-000000000004", nome: "Farmácia", ativo: true },
            { id: "b2222222-3000-0000-0000-000000000004", nome: "Consultas/Exames", ativo: true }
        ]
    },

    // --- DESPESAS: LAZER ---
    {
        _id: "b2222222-0000-0000-0000-000000000005",
        usuarioId: usuarioId,
        nome: "Lazer",
        icone: "mdi-movie",
        cor: "#F1C40F",
        ativo: true,
        subcategorias: [
            { id: "b2222222-1000-0000-0000-000000000005", nome: "Viagens", ativo: true },
            { id: "b2222222-2000-0000-0000-000000000005", nome: "Streaming (Netflix/Spotify)", ativo: true },
            { id: "b2222222-3000-0000-0000-000000000005", nome: "Hobby/Diversão", ativo: true }
        ]
    }
];

db.Categorias.insertMany(categorias);

print("2 - Carga finalizada com sucesso!");