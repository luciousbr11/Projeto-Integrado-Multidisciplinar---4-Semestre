// Main JS for landing page
(function(){
  // Smooth close of menu after clicking a link (on mobile)
  document.querySelectorAll('.navbar-collapse .nav-link').forEach(link => {
    link.addEventListener('click', () => {
      const bsCollapse = bootstrap.Collapse.getInstance(document.getElementById('mainNav'));
      if(bsCollapse && window.innerWidth < 992){
        bsCollapse.hide();
      }
    });
  });

  // Password toggle buttons
  document.querySelectorAll('.input-password').forEach(wrapper => {
    const input = wrapper.querySelector('input[type="password"], input[type="text"]');
    const btn = wrapper.querySelector('.btn-toggle-pass');
    if(input && btn){
      btn.addEventListener('click', () => {
        const visible = input.type === 'text';
        input.type = visible ? 'password' : 'text';
        btn.textContent = visible ? '👁' : '🙈';
      });
    }
  });

  // Simple form validation example (front-end) for cadastro
  const form = document.getElementById('formCadastro');
  if(form){
    form.addEventListener('submit', e => {
      e.preventDefault();
      const pass = form.querySelector('[data-pass]');
      const passConfirm = form.querySelector('[data-pass-confirm]');
      if(pass && passConfirm && pass.value !== passConfirm.value){
        passConfirm.classList.add('is-invalid');
        passConfirm.focus();
        return;
      }
      passConfirm.classList.remove('is-invalid');
      alert('Cadastro enviado (exemplo).');
      form.reset();
    });
  }

  // Login form demo handler
  const loginForm = document.getElementById('formLogin');
  if(loginForm){
    loginForm.addEventListener('submit', e => {
      e.preventDefault();
      alert('Login enviado (exemplo).');
    });
  }

  // Active link highlight based on scroll position (index)
  const sectionIds = ['hero','cadastro','login'];
  const navLinks = document.querySelectorAll('.navbar .nav-link');
  function updateActive(){
    let current = null;
    const scrollPos = window.scrollY + 100; // offset for fixed navbar
    sectionIds.forEach(id => {
      const el = document.getElementById(id);
      if(el){
        if(scrollPos >= el.offsetTop && scrollPos < el.offsetTop + el.offsetHeight){
          current = id;
        }
      }
    });
    navLinks.forEach(link => {
      const href = link.getAttribute('href');
      if(href && href.includes('#')){
        const target = href.split('#')[1];
        if(target === current){
          link.classList.add('active');
        } else {
          link.classList.remove('active');
        }
      }
    });
  }
  window.addEventListener('scroll', updateActive);
  updateActive();

  // Dynamic year
  const yearEl = document.getElementById('year');
  if(yearEl) yearEl.textContent = new Date().getFullYear();
})();
