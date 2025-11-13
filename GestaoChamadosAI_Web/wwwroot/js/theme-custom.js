// Funcionalidades do tema customizado
(function () {
    'use strict';

    // Criar overlay para mobile
    const createOverlay = () => {
        let overlay = document.querySelector('.sidebar-overlay');
        if (!overlay) {
            overlay = document.createElement('div');
            overlay.className = 'sidebar-overlay';
            document.body.appendChild(overlay);
            
            // Fechar sidebar ao clicar no overlay
            overlay.addEventListener('click', () => {
                closeSidebar();
            });
        }
        return overlay;
    };

    const openSidebar = () => {
        const sidebar = document.querySelector('.sidebar');
        const overlay = createOverlay();
        
        sidebar?.classList.add('open');
        overlay?.classList.add('show');
        document.body.style.overflow = 'hidden'; // Prevenir scroll
    };

    const closeSidebar = () => {
        const sidebar = document.querySelector('.sidebar');
        const overlay = document.querySelector('.sidebar-overlay');
        
        sidebar?.classList.remove('open');
        overlay?.classList.remove('show');
        document.body.style.overflow = ''; // Restaurar scroll
    };

    // Toggle da sidebar no mobile
    document.addEventListener('click', (e) => {
        const toggle = e.target.closest('[data-toggle="sidebar"]');
        if (toggle) {
            e.preventDefault();
            const sidebar = document.querySelector('.sidebar');
            if (sidebar?.classList.contains('open')) {
                closeSidebar();
            } else {
                openSidebar();
            }
        }
    });

    // Fechar sidebar ao clicar em um link (mobile)
    document.querySelectorAll('.sidebar .nav-link').forEach((link) => {
        link.addEventListener('click', () => {
            if (window.innerWidth < 992) {
                setTimeout(() => closeSidebar(), 200);
            }
        });
    });

    // Fechar sidebar ao redimensionar para desktop
    window.addEventListener('resize', () => {
        if (window.innerWidth >= 992) {
            closeSidebar();
        }
    });

    // Toggle de colapsar sidebar no desktop
    document.addEventListener('click', (e) => {
        const toggle = e.target.closest('[data-toggle="collapse-sidebar"]');
        if (toggle) {
            document.documentElement.classList.toggle('sidebar-collapsed');
        }
    });

    // Highlight do link ativo na sidebar
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll('.sidebar .nav-link').forEach((link) => {
        const href = link.getAttribute('href');
        if (href) {
            const linkPath = href.split('?')[0].toLowerCase();
            if (currentPath.includes(linkPath) || 
                (linkPath.includes('dashboard') && currentPath === '/')) {
                link.classList.add('active');
            }
        }
    });
})();
