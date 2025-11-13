// Basic app shell: navigation, auth guard, sidebar toggle, page initializers
(function () {

  // Mobile sidebar toggle
  document.addEventListener('click', (e) => {
    const toggle = e.target.closest('[data-toggle="sidebar"]');
    if (toggle) {
      document.querySelector('.sidebar')?.classList.toggle('open');
    }
  });

  // Desktop collapse toggle
  document.addEventListener('click', (e) => {
    const toggle = e.target.closest('[data-toggle="collapse-sidebar"]');
    if (toggle) {
      document.documentElement.classList.toggle('sidebar-collapsed');
    }
  });

  // Active nav highlighting
  const current = location.pathname.split('/').pop();
  document.querySelectorAll('.sidebar .nav-link').forEach((a) => {
    const href = a.getAttribute('href');
    if (href && href.endsWith(current)) a.classList.add('active');
  });

  // Global helpers
  window.formatBadge = function (status) {
    const cls = {
      Aberto: 'Open', 'Em andamento': 'InProgress', Resolvido: 'Resolved', Atrasado: 'Overdue'
    }[status] || 'Open';
    return `<span class="badge badge-status ${cls}">${status}</span>`;
  };
  window.formatId = function (id) { return String(id).padStart(4, '0'); };
  // Parse datas (YYYY-MM-DD ou DD/MM/YYYY)
  window.parseDate = function (s) {
    if (!s) return new Date(0);
    return s.includes('-') ? new Date(s) : new Date(s.split('/').reverse().join('-'));
  };
  // Ordenação: não resolvidos primeiro; dentro do grupo, mais recentes primeiro (ordem de chegada), empates por ID desc
  window.sortTickets = function (a, b) {
    const ar = a.status === 'Resolvido';
    const br = b.status === 'Resolvido';
    if (ar !== br) return ar - br; // false < true => não resolvidos primeiro
    const diff = window.parseDate(b.opened) - window.parseDate(a.opened);
    if (diff !== 0) return diff;
    return (b.id - a.id);
  };

  // Page initializers (run after helpers are defined)
  const pageId = document.body.getAttribute('data-page');
  const map = {
    dashboard: initDashboard,
    chamados: initChamados,
    chamadoDetalhe: initChamadoDetalhe,
    equipe: initEquipe,
    relatorios: initRelatorios,
    configuracoes: initConfiguracoes,
  };
  try { map[pageId]?.(); } catch (e) { console.error('Erro ao iniciar página', pageId, e); }
  // Render notifications after page init
  try { renderNotifications(); } catch (e) { /* noop */ }
})();

// Notificações: simples (mock) – usa chamados atrasados e recentes
function renderNotifications() {
  const menus = document.querySelectorAll('.notif-menu');
  if (!menus.length) return;
  const tickets = (window.mockData?.tickets || []).slice();
  const overdue = tickets.filter(t => t.status === 'Atrasado');
  const recent = tickets.sort((a,b) => window.parseDate(b.opened) - window.parseDate(a.opened)).slice(0,3);
  const items = [];
  overdue.forEach(t => items.push({ icon: 'exclamation-triangle-fill text-danger', text: `Atrasado • ${t.title}` , href: `chamado-detalhe.html?id=${t.id}` }));
  recent.forEach(t => items.push({ icon: 'ticket-detailed-fill text-primary', text: `Novo • ${t.title}`, href: `chamado-detalhe.html?id=${t.id}` }));
  const html = items.length ? (
      `<div class="list-group list-group-flush" style="min-width:320px; max-height:360px; overflow:auto;">
        ${items.map(i => `<a class="list-group-item list-group-item-action d-flex align-items-center gap-2" href="${i.href}">
          <i class="bi ${i.icon}"></i><span>${i.text}</span>
        </a>`).join('')}
      </div>
      <div class="p-2 text-center"><button class="btn btn-sm btn-outline-secondary" data-clear-notifs>Limpar tudo</button></div>`
  ) : '<div class="p-3 text-muted">Sem notificações</div>';
  menus.forEach(m => m.innerHTML = html);
  const count = items.length;
  document.querySelectorAll('.notif-badge').forEach(b => b.textContent = String(count));
    // Handler para limpar tudo
    menus.forEach(m => {
      m.querySelector('[data-clear-notifs]')?.addEventListener('click', (e) => {
        e.preventDefault();
        window._notifCleared = true;
        // Após limpar, mostrar vazio e zerar badges
        m.innerHTML = '<div class="p-3 text-muted">Sem notificações</div>';
        document.querySelectorAll('.notif-badge').forEach(b => b.textContent = '0');
      });
    });
}

// Dashboard init
function initDashboard() {
  // KPI values from mock data
  const data = window.mockData.tickets;
  const open = data.filter(d => d.status === 'Aberto').length;
  const progress = data.filter(d => d.status === 'Em andamento').length;
  const resolved = data.filter(d => d.status === 'Resolvido').length;
  const overdue = data.filter(d => d.status === 'Atrasado').length;
  document.getElementById('kpi-open').textContent = open;
  document.getElementById('kpi-progress').textContent = progress;
  document.getElementById('kpi-resolved').textContent = resolved;
  document.getElementById('kpi-overdue').textContent = overdue;

  if (window.Chart) {
    const ctx = document.getElementById('statusChart');
    new Chart(ctx, {
      type: 'doughnut',
      data: {
        labels: ['Aberto', 'Em andamento', 'Resolvido', 'Atrasado'],
        datasets: [{
          data: [open, progress, resolved, overdue],
          backgroundColor: ['#0d6efd', '#f59e0b', '#22c55e', '#ef4444'],
        }]
      },
      options: { responsive: true, plugins: { legend: { position: 'bottom' } } }
    });
  }

  // Latest table
  const tbody = document.querySelector('#latest-tickets tbody');
  tbody.innerHTML = '';
  data.slice().sort(window.sortTickets).slice(0, 6).forEach(t => {
    const tr = document.createElement('tr');
    tr.innerHTML = `<td>${formatId(t.id)}</td><td>${t.title}</td><td>${t.client}</td><td>${formatBadge(t.status)}</td>`;
    tr.addEventListener('click', () => location.href = `chamado-detalhe.html?id=${t.id}`);
    tbody.appendChild(tr);
  });
}

// Chamados list init
function initChamados() {
  const data = window.mockData.tickets;
  const tbody = document.querySelector('#tickets-table tbody');
  const filters = {
    status: document.getElementById('filter-status'),
    client: document.getElementById('filter-client'),
    assignee: document.getElementById('filter-assignee'),
  date: document.getElementById('filter-date'),
  order: document.getElementById('sort-order')
  };

  // Populate filter options (safe creation)
  const clients = [...new Set(data.map(d => d.client))];
  clients.forEach(c => { const opt = document.createElement('option'); opt.value = c; opt.textContent = c; filters.client.appendChild(opt); });
  const assignees = [...new Set(data.map(d => d.assignee))];
  assignees.forEach(u => { const opt = document.createElement('option'); opt.value = u; opt.textContent = u; filters.assignee.appendChild(opt); });

  const apply = () => {
    tbody.innerHTML = '';
    data.slice()
        .filter(d => !filters.status.value || d.status === filters.status.value)
        .filter(d => !filters.client.value || d.client === filters.client.value)
        .filter(d => !filters.assignee.value || d.assignee === filters.assignee.value)
        .filter(d => !filters.date.value || d.opened.startsWith(filters.date.value))
        .sort((a,b) => {
          // Resolvido por último
          const ar = a.status === 'Resolvido';
          const br = b.status === 'Resolvido';
          if (ar !== br) return ar - br;
          // Ordenação por chegada
          const da = window.parseDate(a.opened), db = window.parseDate(b.opened);
          const asc = (filters.order?.value === 'asc');
          const diff = asc ? (da - db) : (db - da);
          if (diff !== 0) return diff;
          return asc ? (a.id - b.id) : (b.id - a.id);
        })
        .forEach(t => {
          const tr = document.createElement('tr');
          const openedBr = (t.opened.includes('-') ? t.opened.split('-').reverse().join('/') : t.opened);
          tr.innerHTML = `<td>${formatId(t.id)}</td><td>${t.title}</td><td>${t.client}</td><td>${formatBadge(t.status)}</td><td>${t.priority}</td><td>${t.assignee}</td><td>${openedBr}</td>`;
          tr.addEventListener('click', () => location.href = `chamado-detalhe.html?id=${t.id}`);
          tbody.appendChild(tr);
        });
  };
  Object.values(filters).forEach(el => el?.addEventListener('change', apply));
  document.getElementById('btn-search')?.addEventListener('click', apply);
  apply();

  // New ticket (mock)
  document.getElementById('form-new-ticket')?.addEventListener('submit', (e) => {
    e.preventDefault();
    const form = e.target;
  const id = (data.length ? Math.max(...data.map(d => d.id)) : 0) + 1;
    data.unshift({
      id,
      title: form.title.value,
      client: form.client.value,
      status: 'Aberto',
      priority: form.priority.value,
      assignee: form.assignee.value || '-',
      opened: new Date().toISOString().slice(0,10)
    });
    const modalEl = document.getElementById('modalNewTicket');
    if (modalEl) {
      const modal = bootstrap.Modal.getInstance(modalEl);
      modal?.hide();
    }
    form.reset?.();
    apply();
  });
}

// Chamado detalhe
function initChamadoDetalhe() {
  const params = new URLSearchParams(location.search);
  const id = Number(params.get('id')) || window.mockData.tickets[0]?.id;
  const ticket = window.mockData.tickets.find(t => t.id === id) || window.mockData.tickets[0];
  if (!ticket) return;

  document.getElementById('ticket-title').textContent = ticket.title;
  document.getElementById('ticket-client').textContent = ticket.client;
  document.getElementById('select-status').value = ticket.status;
  document.getElementById('select-priority').value = ticket.priority;
  const selAssign = document.getElementById('select-assignee');
  selAssign.value = ticket.assignee;

  // CLAIM (Assumir) - gerente pode assumir qualquer chamado
  const me = (JSON.parse(localStorage.getItem('profile')||'{}').name || 'Gerente');
  // Garante que o usuário apareça na lista
  if(![...selAssign.options].some(o=>o.value===me)) {
    const opt=document.createElement('option'); opt.value=me; opt.textContent=me; selAssign.appendChild(opt);
  }
  const btnClaim = document.getElementById('btn-claim');
  const ensureHistoryArray = () => { ticket.history = ticket.history || []; };
  const refreshClaimVisibility = () => {
    if(!ticket.assignee || ticket.assignee.toLowerCase() !== me.toLowerCase()) btnClaim?.classList.remove('d-none');
    else btnClaim?.classList.add('d-none');
  };
  refreshClaimVisibility();
  btnClaim?.addEventListener('click', ()=>{
    ticket.assignee = me;
    selAssign.value = me;
    ensureHistoryArray();
    ticket.history.unshift({ author: 'Você', text: 'assumiu o chamado', time: new Date().toLocaleString() });
    renderTimeline();
    refreshClaimVisibility();
  });

  const timeline = document.getElementById('timeline');
  const renderTimeline = () => {
    timeline.innerHTML = '';
    ticket.history.forEach(h => {
      const div = document.createElement('div');
      div.className = 'timeline-item';
      div.innerHTML = `<div><strong>${h.author}</strong> ${h.text}</div><div class="time">${h.time}</div>`;
      timeline.appendChild(div);
    });
  };
  renderTimeline();

  // Injeção automática de mensagens da IA ao abrir (somente uma vez por ticket)
  (function autoIaIntake(){
    try {
      const has = (ticket.history||[]).some(h => (h.text||'').toLowerCase().includes('solicitação inicial'));
      if (has) return;
      const now = new Date().toLocaleString();
      const initialDesc = (ticket.history||[]).find(h => (h.author||'').toLowerCase().includes('cliente') || (h.text||'').toLowerCase().includes('descrição'))?.text || '-';
      ticket.history = ticket.history || [];
      ticket.history.unshift({ author: 'IA', text: `Encaminhado para ${ticket.assignee || 'NÃO DEFINIDO'}`, time: now });
      ticket.history.unshift({ author: 'IA', text: 'Recebi a solicitação do usuário e coletei as informações iniciais.', time: now });
      ticket.history.unshift({ author: 'IA', text: `Solicitação inicial — Nome: ${ticket.client || '-'}; Título: ${ticket.title || '-'}; Descrição: ${initialDesc || '-'}`, time: now });
      renderTimeline();
      // Mensagem no chat para visibilidade
      (ticket.chat = ticket.chat || []).unshift({ role: 'system', author: 'IA', time: now, text: 'Nova solicitação recebida e encaminhada à equipe.' });
    } catch(e) { /* noop */ }
  })();

  // Render chat estilo mensageria
  const chatBox = document.getElementById('chat-messages');
  if (chatBox) {
    const renderChat = () => {
      chatBox.innerHTML = '';
      (ticket.chat || []).forEach(m => {
        const wrap = document.createElement('div');
        wrap.className = 'chat-msg ' + (m.role === 'agent' ? 'me' : 'other');
        wrap.innerHTML = `<div class="bubble"><div class="text">${m.text}</div><div class="time">${m.time}</div></div>`;
        chatBox.appendChild(wrap);
      });
      chatBox.scrollTop = chatBox.scrollHeight;
    };
    renderChat();

    const form = document.getElementById('chat-form');
    const input = document.getElementById('chat-input');
    form?.addEventListener('submit', (e) => {
      e.preventDefault();
      const text = input.value.trim(); if (!text) return;
      (ticket.chat = ticket.chat || []).push({ role: 'agent', author: 'Você', time: new Date().toLocaleString(), text });
      input.value = '';
      renderChat();
    });
  }

  // Dropdown updates
  document.getElementById('select-status').addEventListener('change', (e) => {
    ticket.status = e.target.value;
    ticket.history.unshift({ author: 'Você', text: `alterou o status para ${ticket.status}`, time: new Date().toLocaleString() });
    renderTimeline();
  });
  document.getElementById('select-priority').addEventListener('change', (e) => {
    ticket.priority = e.target.value;
    ticket.history.unshift({ author: 'Você', text: `alterou a prioridade para ${ticket.priority}`, time: new Date().toLocaleString() });
    renderTimeline();
  });
  selAssign.addEventListener('change', (e) => {
    ticket.assignee = e.target.value;
    ticket.history.unshift({ author: 'Você', text: `atribuiu para ${ticket.assignee}`, time: new Date().toLocaleString() });
    renderTimeline();
    refreshClaimVisibility();
  });

  // Reply box
  document.getElementById('form-reply').addEventListener('submit', (e) => {
    e.preventDefault();
    const text = e.target.message.value.trim();
    if (!text) return;
    ticket.history.unshift({ author: 'Você', text, time: new Date().toLocaleString() });
    e.target.reset();
    renderTimeline();
  });

  // Actions
  document.getElementById('btn-close').addEventListener('click', () => {
    ticket.status = 'Resolvido';
    document.getElementById('select-status').value = ticket.status;
    ticket.history.unshift({ author: 'Você', text: 'encerrou o chamado', time: new Date().toLocaleString() });
    renderTimeline();
  });
  document.getElementById('btn-reopen').addEventListener('click', () => {
    ticket.status = 'Aberto';
    document.getElementById('select-status').value = ticket.status;
    ticket.history.unshift({ author: 'Você', text: 'reabriu o chamado', time: new Date().toLocaleString() });
    renderTimeline();
  });
}


// Equipe
function initEquipe() {
  const tbody = document.querySelector('#team-table tbody');
  const data = window.mockData.team;
  const render = () => {
    tbody.innerHTML = '';
    data.forEach((u, idx) => {
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${u.name}</td><td>${u.email}</td><td>${u.role}</td><td>
        <div class="form-check form-switch d-inline-block">
          <input class="form-check-input" type="checkbox" ${u.active ? 'checked' : ''} data-toggle-active="${idx}">
        </div></td>`;
      tbody.appendChild(tr);
    });
  };
  render();

  document.getElementById('form-new-user')?.addEventListener('submit', (e) => {
    e.preventDefault(); const f = e.target;
    // NOTE: A senha é armazenada em texto puro apenas para demonstração em memória.
    // Em um ambiente real, nunca armazene ou transmita senhas sem hashing/sal.
    data.unshift({ name: f.name.value, email: f.email.value, role: f.role.value, password: f.password.value, active: true });
    bootstrap.Modal.getInstance(document.getElementById('modalNewUser'))?.hide(); f.reset(); render();
  });

  tbody.addEventListener('change', (e) => {
    const t = e.target.closest('[data-toggle-active]'); if (!t) return;
    data[Number(t.dataset.toggleActive)].active = t.checked; 
  });

  // Toggle mostrar/ocultar senha
  const pwdBtn = document.querySelector('#form-new-user [data-toggle-password]');
  const pwdInput = document.querySelector('#form-new-user input[name="password"]');
  pwdBtn?.addEventListener('click', () => {
    if (!pwdInput) return;
    const isHidden = pwdInput.type === 'password';
    pwdInput.type = isHidden ? 'text' : 'password';
    pwdBtn.querySelector('i')?.classList.toggle('bi-eye');
    pwdBtn.querySelector('i')?.classList.toggle('bi-eye-slash');
  });
}

// Relatórios
function initRelatorios() {
  const { tickets } = window.mockData;
  const apply = () => {
    const byStatus = { 'Aberto':0, 'Em andamento':0, 'Resolvido':0, 'Atrasado':0 };
    const byAgent = {};
    const rows = [];
    tickets.forEach(t => {
      byStatus[t.status] = (byStatus[t.status]||0) + 1;
      byAgent[t.assignee] = (byAgent[t.assignee]||0) + 1;
      rows.push([window.formatId(t.id), t.title, t.client, t.assignee, t.status, t.priority, t.opened]);
    });
    if (window.Chart) {
      renderBar('chart-status', Object.keys(byStatus), Object.values(byStatus));
      renderBar('chart-agent', Object.keys(byAgent), Object.values(byAgent));
    }
    const tbody = document.querySelector('#reports-table tbody');
    tbody.innerHTML = '';
    rows.forEach(r => {
      const tr = document.createElement('tr');
      tr.innerHTML = `<td>${r[0]}</td><td>${r[1]}</td><td>${r[2]}</td><td>${r[3]}</td><td>${r[4]}</td><td>${r[5]}</td><td>${r[6]}</td>`;
      tbody.appendChild(tr);
    });
  };
  document.getElementById('btn-apply')?.addEventListener('click', apply);
  apply();

  document.getElementById('btn-export-excel')?.addEventListener('click', () => {
    const rows = [['ID','Título','Nome','Atendente','Status','Prioridade','Abertura']];
    document.querySelectorAll('#reports-table tbody tr').forEach(tr => {
      rows.push(Array.from(tr.children).map(td => '"' + td.textContent.replaceAll('"','""') + '"'));
    });
    const blob = new Blob([rows.map(r => r.join(',')).join('\n')], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const a = Object.assign(document.createElement('a'), { href: url, download: 'relatorio_chamados.csv' });
    a.click(); URL.revokeObjectURL(url);
  });

  document.getElementById('btn-export-pdf')?.addEventListener('click', () => {
    if (!window.jspdf || !window.jspdf.jsPDF || !window.jspdf.autoTable) {
      alert('Biblioteca jsPDF/autoTable não carregada.'); return;
    }
    const doc = new window.jspdf.jsPDF();
    const head = [['ID','Título','Nome','Atendente','Status','Prioridade','Abertura']];
    const body = [];
    document.querySelectorAll('#reports-table tbody tr').forEach(tr => {
      body.push(Array.from(tr.children).map(td => td.textContent));
    });
    doc.text('Relatório de Chamados', 14, 14);
    doc.autoTable({ head, body, startY: 20 });
    doc.save('relatorio_chamados.pdf');
  });
}

function renderBar(canvasId, labels, data) {
  const ctx = document.getElementById(canvasId);
  new Chart(ctx, { type: 'bar', data: { labels, datasets: [{ label: '#', data, backgroundColor: '#0d6efd' }] }, options: { responsive: true, plugins: { legend: { display: false } } } });
}

// Configurações
function initConfiguracoes() {
  const f = document.getElementById('form-settings');
  f?.addEventListener('submit', (e) => {
    e.preventDefault();
    alert('Configurações salvas (mock).');
  });
}
