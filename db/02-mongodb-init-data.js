db = db.getSiblingDB('FinancasDb');

print("--- INICIANDO CARGA COMPLETA: Usuário, Categorias e Transações ---");

var usuarioId = UUID("11111111-1111-1111-1111-111111111111");

// 1. LIMPEZA E CRIAÇÃO DO USUÁRIO
db.Usuarios.deleteMany({ _id: usuarioId });
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

// 2. CATEGORIAS
db.Categorias.deleteMany({ usuarioId: usuarioId });
var categorias = [];

// --- RECEITAS (TIPO R) ---
var receitasList = [
    { id: "1", nome: "SALARIO", icone: "Banknote" },
    { id: "2", nome: "INVESTIMENTOS", icone: "TrendingUp" },
    { id: "3", nome: "FREELANCE", icone: "Laptop" },
    { id: "4", nome: "COMISSAO", icone: "BadgePercent" },
    { id: "5", nome: "REEMBOLSO", icone: "Undo2" },
    { id: "6", nome: "VENDA DE BENS", icone: "ShoppingBag" }
];

receitasList.forEach(item => {
    categorias.push({
        _id: UUID("a1111111-0000-0000-0000-00000000000" + item.id),
        usuarioId: usuarioId, 
        nome: item.nome, 
        tipo: "R", 
        icone: item.icone,
        categoriaPaiId: null, 
        ativo: true
    });
});

// --- FUNÇÃO PARA DESPESAS (Geração de UUIDs Padronizados) ---
function addDespesa(paiNome, paiIcon, paiIdPrefix, filhas) {
    var prefix = "b" + paiIdPrefix.padStart(7, '0'); // Ex: b0000009
    var paiId = UUID(prefix + "-0000-0000-0000-000000000000");
    
    categorias.push({
        _id: paiId, 
        usuarioId: usuarioId, 
        nome: paiNome, 
        tipo: "D", 
        icone: paiIcon,
        categoriaPaiId: null, 
        ativo: true
    });

    filhas.forEach((filha, i) => {
        // Gera IDs como b0000009-0001..., b0000009-0002...
        categorias.push({
            _id: UUID(prefix + "-" + (i + 1).toString().padStart(4, '0') + "-0000-0000-000000000000"),
            usuarioId: usuarioId, 
            nome: filha.nome, 
            tipo: "D", 
            icone: filha.icone,
            categoriaPaiId: paiId, 
            ativo: true
        });
    });
}

// Carga das Despesas
addDespesa("ALIMENTAÇÃO", "Utensils", "1", [{nome:"DOCERIA",icone:"CakeSlice"},{nome:"FRUTARIA",icone:"Apple"},{nome:"IFOOD",icone:"Bike"},{nome:"MERCADO",icone:"ShoppingCart"},{nome:"RESTAURANTE",icone:"Soup"},{nome:"OUTROS",icone:"MoreHorizontal"}]);
addDespesa("ANIMAIS", "PawPrint", "2", [{nome:"BANHO E TOSA",icone:"Scissors"},{nome:"RAÇÃO",icone:"Dog"},{nome:"VETERINARIO",icone:"Stethoscope"}]);
addDespesa("AUTOMÓVEL", "Car", "3", [{nome:"COMBUSTIVEL",icone:"Fuel"},{nome:"MANUTENÇÃO",icone:"Wrench"},{nome:"UBER",icone:"Navigation"},{nome:"SEGURO",icone:"ShieldCheck"}]);
addDespesa("BANCO", "Landmark", "4", [{nome:"JUROS",icone:"Percent"},{nome:"TARIFAS",icone:"Receipt"}]);
addDespesa("BEM-ESTAR", "Sparkles", "5", [{nome:"ACADEMIA",icone:"Dumbbell"},{nome:"ESTETICA",icone:"Flower2"}]);
addDespesa("DIVERSÃO", "Gamepad2", "6", [{nome:"CINEMA",icone:"Film"},{nome:"VIAGEM",icone:"Plane"},{nome:"FESTA",icone:"PartyPopper"}]);
addDespesa("EDUCAÇÃO", "GraduationCap", "8", [{nome:"FACULDADE",icone:"School"},{nome:"CURSO",icone:"Library"}]);
addDespesa("MORADIA", "Home", "9", [{nome:"ALUGUEL",icone:"Key"},{nome:"LUZ",icone:"Zap"},{nome:"AGUA",icone:"Droplets"},{nome:"INTERNET",icone:"Wifi"}]);
addDespesa("SAÚDE", "HeartPulse", "10", [{nome:"CONVENIO",icone:"BriefcaseMedical"},{nome:"MÉDICO",icone:"UserRound"},{nome:"REMEDIO",icone:"Pill"}]);

db.Categorias.insertMany(categorias);

// 3. TRANSAÇÕES (CORRIGIDAS PARA BATER COM OS IDS DAS CATEGORIAS)
db.Transacoes.deleteMany({ usuarioId: usuarioId });
var transacoes = [];
function criarData(ano, mes, dia) { return new ISODate(ano + "-" + (mes < 10 ? "0" + mes : mes) + "-" + (dia < 10 ? "0" + dia : dia) + "T12:00:00Z"); }

for (var m = 1; m <= 5; m++) {
    var ano = 2026;

    // RECEITA (SALÁRIO - ID 1)
    transacoes.push({
        _id: UUID(), 
        usuarioId: usuarioId, 
        descricao: "Salário Mensal", 
        valor: NumberDecimal("8500.00"),
        data: criarData(ano, m, 5), 
        tipo: "R",
        categoriaId: UUID("a1111111-0000-0000-0000-000000000001"), 
        categoriaNome: "SALARIO", 
        categoriaIcone: "Banknote",
        categoriaPaiId: null, 
        dataCriacao: new ISODate()
    });

    // ALUGUEL (Pai 9, Filha 1 -> ID 0001)
    transacoes.push({
        _id: UUID(), 
        usuarioId: usuarioId, 
        descricao: "Pagamento Aluguel", 
        valor: NumberDecimal("2600.00"),
        data: criarData(ano, m, 10), 
        tipo: "D",
        categoriaId: UUID("b0000009-0001-0000-0000-000000000000"), 
        categoriaNome: "ALUGUEL", 
        categoriaIcone: "Key",
        categoriaPaiId: UUID("b0000009-0000-0000-0000-000000000000"), 
        categoriaPaiNome: "MORADIA", 
        categoriaPaiIcone: "Home",
        dataCriacao: new ISODate()
    });

    // MERCADO (Pai 1, Filha 4 -> ID 0004)
    transacoes.push({
        _id: UUID(), 
        usuarioId: usuarioId, 
        descricao: "Compras Mensais", 
        valor: NumberDecimal((Math.random() * (900 - 600) + 600).toFixed(2)),
        data: criarData(ano, m, 15), 
        tipo: "D",
        categoriaId: UUID("b0000001-0004-0000-0000-000000000000"), 
        categoriaNome: "MERCADO", 
        categoriaIcone: "ShoppingCart",
        categoriaPaiId: UUID("b0000001-0000-0000-0000-000000000000"), 
        categoriaPaiNome: "ALIMENTAÇÃO", 
        categoriaPaiIcone: "Utensils",
        dataCriacao: new ISODate()
    });

    // INTERNET (Pai 9, Filha 4 -> ID 0004)
    transacoes.push({
        _id: UUID(), 
        usuarioId: usuarioId, 
        descricao: "Fibra Óptica", 
        valor: NumberDecimal("129.90"),
        data: criarData(ano, m, 18), 
        tipo: "D",
        categoriaId: UUID("b0000009-0004-0000-0000-000000000000"), 
        categoriaNome: "INTERNET", 
        categoriaIcone: "Wifi",
        categoriaPaiId: UUID("b0000009-0000-0000-0000-000000000000"), 
        categoriaPaiNome: "MORADIA", 
        categoriaPaiIcone: "Home",
        dataCriacao: new ISODate()
    });
}

db.Transacoes.insertMany(transacoes);

print("--- CARGA FINALIZADA COM SUCESSO ---");