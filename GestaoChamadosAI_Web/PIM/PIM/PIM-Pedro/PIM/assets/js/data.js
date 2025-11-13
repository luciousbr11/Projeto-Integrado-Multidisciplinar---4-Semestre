// Mock data for the system
window.mockData = {
  tickets: [
    // Ticket de exemplo solicitado
  { id: 1, title: 'Erro ao acessar o sistema', client: 'João Silva', status: 'Aberto', priority: 'Alta', assignee: 'Não atribuído', opened: '2025-09-10',
      history: [
        { author: 'João Silva (cliente)', text: 'Chamado aberto por João Silva.', time: '10/09/2025 - 09:15' },
        { author: 'João Silva (cliente)', text: 'Descrição inicial: “Não consigo acessar o sistema, aparece erro 500.”', time: '10/09/2025 - 09:15' }
      ],
      chat: [
        { role: 'client', author: 'João Silva', time: '10/09/2025 - 09:15', text: 'Não consigo acessar o sistema, aparece erro 500.' },
        { role: 'agent', author: 'Atendente Maria', time: '10/09/2025 - 09:20', text: 'Olá João, estamos verificando o problema. Você consegue me enviar um print?' },
        { role: 'client', author: 'João Silva', time: '10/09/2025 - 09:22', text: 'Consigo sim, vou anexar aqui.' }
      ]
    },
    { id: 1024, title: 'Erro ao acessar sistema', client: 'Acme S.A.', status: 'Aberto', priority: 'Alta', assignee: 'João Silva', opened: '2025-09-01', history: [ { author: 'Cliente', text: 'Reporte de erro 500', time: '2025-09-01 09:12' } ] },
  { id: 1023, title: 'Integração chat externo', client: 'Beta Ltda', status: 'Em andamento', priority: 'Média', assignee: 'Maria Souza', opened: '2025-08-30', history: [ { author: 'Atendente', text: 'Aguardando retorno do cliente', time: '2025-09-02 11:40' } ] },
    { id: 1022, title: 'Reset de senha', client: 'Gamma Corp', status: 'Resolvido', priority: 'Baixa', assignee: 'João Silva', opened: '2025-08-28', history: [ { author: 'Atendente', text: 'Senha redefinida', time: '2025-08-28 16:10' } ] },
    { id: 1021, title: 'Falha em emissão de NF', client: 'Acme S.A.', status: 'Atrasado', priority: 'Alta', assignee: 'Ana Lima', opened: '2025-08-20', history: [ { author: 'Atendente', text: 'Investigando causa raiz', time: '2025-08-21 10:00' } ] },
    { id: 1020, title: 'Bug no relatório de vendas', client: 'Delta Co', status: 'Aberto', priority: 'Média', assignee: 'Pedro Alves', opened: '2025-08-25', history: [ { author: 'Cliente', text: 'Dados divergentes', time: '2025-08-25 08:21' } ] },
    { id: 1019, title: 'Dúvida sobre API', client: 'Omega Inc', status: 'Em andamento', priority: 'Baixa', assignee: 'Maria Souza', opened: '2025-09-03', history: [ { author: 'Atendente', text: 'Enviado manual de API', time: '2025-09-03 13:33' } ] }
    ,
    { id: 1025, title: 'Erro de login no app mobile', client: 'Zeta Tech', status: 'Aberto', priority: 'Alta', assignee: 'João Silva', opened: '2025-09-12', history: [ { author: 'Cliente', text: 'Usuários não conseguem autenticar', time: '2025-09-12 08:10' } ] },
    { id: 1026, title: 'Acesso negado ao módulo Fiscal', client: 'Acme S.A.', status: 'Em andamento', priority: 'Urgente', assignee: 'Maria Souza', opened: '2025-09-11', history: [ { author: 'Atendente', text: 'Revisando permissões', time: '2025-09-11 10:42' } ] },
    { id: 1027, title: 'Lentidão geral no sistema', client: 'Beta Ltda', status: 'Aberto', priority: 'Média', assignee: 'Ana Lima', opened: '2025-09-09', history: [ { author: 'Cliente', text: 'Respostas acima de 5s', time: '2025-09-09 14:05' } ] },
    { id: 1028, title: 'Erro ao exportar CSV', client: 'Omega Inc', status: 'Resolvido', priority: 'Baixa', assignee: 'Pedro Alves', opened: '2025-08-18', history: [ { author: 'Atendente', text: 'Corrigido delimitador', time: '2025-08-19 09:11' } ] },
    { id: 1029, title: 'Falha de backup', client: 'Delta Co', status: 'Atrasado', priority: 'Urgente', assignee: 'João Silva', opened: '2025-08-15', history: [ { author: 'Atendente', text: 'Backup não executou na janela', time: '2025-08-16 02:10' } ] },
    { id: 1030, title: 'Configuração de e-mail SMTP', client: 'Gamma Corp', status: 'Em andamento', priority: 'Média', assignee: 'Pedro Alves', opened: '2025-09-05', history: [ { author: 'Atendente', text: 'Aguardando credenciais do cliente', time: '2025-09-05 15:30' } ] },
    { id: 1031, title: 'Notificação não recebida', client: 'Zeta Tech', status: 'Aberto', priority: 'Baixa', assignee: 'Ana Lima', opened: '2025-09-07', history: [ { author: 'Cliente', text: 'Alguns usuários não recebem e-mail', time: '2025-09-07 09:50' } ] },
    { id: 1032, title: 'Integração ERP - mapeamento', client: 'Acme S.A.', status: 'Em andamento', priority: 'Alta', assignee: 'Maria Souza', opened: '2025-09-02', history: [ { author: 'Atendente', text: 'Validando campos obrigatórios', time: '2025-09-03 11:25' } ] },
    { id: 1033, title: 'Erro 404 em página de ajuda', client: 'Beta Ltda', status: 'Resolvido', priority: 'Baixa', assignee: 'João Silva', opened: '2025-08-22', history: [ { author: 'Atendente', text: 'Rota corrigida e publicada', time: '2025-08-23 17:40' } ] },
    { id: 1034, title: 'Atualização de plano', client: 'Omega Inc', status: 'Aberto', priority: 'Média', assignee: 'Pedro Alves', opened: '2025-09-10', history: [ { author: 'Cliente', text: 'Solicitou upgrade para Plano Pro', time: '2025-09-10 12:00' } ] },
    { id: 1035, title: 'Dashboard sem dados', client: 'Delta Co', status: 'Atrasado', priority: 'Alta', assignee: 'Maria Souza', opened: '2025-08-12', history: [ { author: 'Atendente', text: 'ETL falhou na madrugada', time: '2025-08-13 06:20' } ] },
    { id: 1036, title: 'API retornando 401', client: 'Gamma Corp', status: 'Em andamento', priority: 'Alta', assignee: 'João Silva', opened: '2025-09-06', history: [ { author: 'Atendente', text: 'Token expirado, revisar refresh', time: '2025-09-06 10:05' } ] }
  ],
  clients: [
    { name: 'Acme S.A.', company: 'Acme', email: 'contato@acme.com', phone: '(11) 99999-0000', tickets: 23 },
    { name: 'Beta Ltda', company: 'Beta', email: 'suporte@beta.com', phone: '(21) 98888-1111', tickets: 12 },
    { name: 'Gamma Corp', company: 'Gamma', email: 'ti@gamma.com', phone: '(31) 97777-2222', tickets: 8 }
  ],
  team: [
    { name: 'João Silva', email: 'joao@empresa.com', role: 'Atendente', active: true },
    { name: 'Maria Souza', email: 'maria@empresa.com', role: 'Gerente', active: true },
    { name: 'Ana Lima', email: 'ana@empresa.com', role: 'Atendente', active: false },
    { name: 'Pedro Alves', email: 'pedro@empresa.com', role: 'Atendente', active: true }
  ]
};
