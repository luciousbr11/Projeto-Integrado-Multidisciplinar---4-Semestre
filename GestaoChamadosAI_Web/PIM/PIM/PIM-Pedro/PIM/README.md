# PIM • Service Desk (estilo Tiflux)

Sistema web estático para gerenciamento de chamados, inspirado no layout do Tiflux. Construído com HTML, CSS, JavaScript e Bootstrap (bundle local já incluso em `bootstrap/`).

## Páginas
- `login.html`: Tela de login centralizada, redireciona para o dashboard.
- `dashboard.html`: Sidebar fixa, cabeçalho com usuário, cards de indicadores, gráfico de status e últimos chamados.
- `chamados.html`: Listagem com filtros (status, cliente, responsável, data), modal de novo chamado e clique para detalhes.
- `chamado-detalhe.html`: Informações do chamado, dropdowns (status, prioridade, responsável), timeline, responder, encerrar/reabrir.
- `clientes.html`: CRUD simples com modais (novo/editar), colunas Nome/Empresa/Contato/Qtd.
- `equipe.html`: Usuários com cargos (Admin/Atendente/Cliente) e ativar/desativar.
- `relatorios.html`: Filtros por período, gráficos (status, cliente, atendente) e exportar Excel (CSV) / PDF (jsPDF + autoTable).
- `configuracoes.html`: Dados da empresa, SLA, templates de e-mail e integrações.

## Estrutura
- `bootstrap/`: CSS/JS do Bootstrap (local).
- `assets/css/theme.css`: Tema visual (sidebar fixa, header, cards, timeline, responsivo).
- `assets/js/data.js`: Mock de dados (tickets/clientes/equipe).
- `assets/js/app.js`: Interações globais e inicialização por página.
- `assets/img/`: Imagens (adicione `logo.png` opcionalmente).

## Como executar
Abra o `login.html` no navegador ou sirva a pasta com um servidor estático.

PowerShell (opcional):

```powershell
# Com Python 3
python -m http.server 8080
# Acesse http://localhost:8080/login.html
```

## Observações
- Autenticação é mock (usa `localStorage`).
- Layout responsivo: sidebar vira menu hambúrguer no mobile; no desktop é possível recolher a sidebar.
- Exports: Excel usa CSV; PDF usa jsPDF + autoTable via CDN.

## Customização
- Ajuste cores, espaços e comportamentos em `assets/css/theme.css`.
- Substitua/integre os mocks em `assets/js/data.js` por APIs reais.
