/*document.addEventListener('DOMContentLoaded', function () {
    window.addEventListener('scroll', function () {
        var heade = document.getElementById('header-top')
        var scrollTop = window.ScrollY;
        var maxScroll = 250;

        var opacity = Math.min(scrollTop / maxScroll, 1);
        header.style.backgroundColor = `rgba(0, 0, 139, ${opacity})`;
    });
});
*/
document.addEventListener('DOMContentLoaded', function () {
    window.addEventListener('scroll', function () {
        var header = document.getElementById('header-top');
        var buttonHeader = document.querySelector('.button-header');
        var menu = document.querySelector('.menu');
        var logo = document.querySelector('.logo');
        var scrollTop = window.scrollY;
        var maxScroll = 250;

        if (scrollTop > maxScroll) {
            /* header.style.backgroundColor = 'orange'; */
            header.style.backgroundColor = 'transparent';
            if (buttonHeader) if (buttonHeader) buttonHeader.classList.add('hidden-element');
            if (menu) menu.classList.add('hidden-element');
            if (logo) logo.classList.add('hidden-element');
        }
        
        else {
            /* header.style.backgroundColor = 'transparent'; */
                header.style.backgroundColor = 'orange';
                if (buttonHeader) {
                    buttonHeader.classList.remove('hidden-button');
                    if (buttonHeader) buttonHeader.classList.remove('hidden-element');
                    if (menu) menu.classList.remove('hidden-element');
                    if (logo) logo.classList.remove('hidden-element');
                }
        }
    });
});

document.addEventListener('DOMContentLoaded', function () {
    let currentSlide = 0;
    const cardWrapper = document.querySelector('.card-wrapper');
    const cards = document.querySelectorAll('.card');
    const cardsPerRow = 3;
    const totalSlides = Math.ceil(cards.length / cardsPerRow);

    function next() {
        if (currentSlide < totalSlides - 1) {
            currentSlide++;
            cardWrapper.style.transform = `translateX(-${currentSlide * 100}%)`;
        }
    }

    function prev() {
        if (currentSlide > 0) {
            currentSlide--;
            cardWrapper.style.transform = `translateX(-${currentSlide * 100}%)`;
        }
    }
    
    document.querySelector('.arrow.right').addEventListener('click', next);
    document.querySelector('.arrow.left').addEventListener('click', prev);

    /*Для мобилок*/
});
document.addEventListener('DOMContentLoaded', function () {
    const hamburger = document.getElementById('hamburger');
    const sideMenu = document.getElementById('side-menu');
    const sideMenuOverlay = document.getElementById('side-menu-overlay');

    if (hamburger && sideMenu) {
        hamburger.addEventListener('click', function () {
            sideMenu.classList.add('active');
            if (sideMenuOverlay) sideMenuOverlay.classList.add('active');
            document.body.style.overflow = 'hidden'; // Блокируем скролл
        });

        // Закрытие меню при клике на overlay
        if (sideMenuOverlay) {
            sideMenuOverlay.addEventListener('click', function () {
                sideMenu.classList.remove('active');
                sideMenuOverlay.classList.remove('active');
                document.body.style.overflow = ''; // Разблокируем скролл
            });
        }

        // Закрытие меню при клике на ссылки
        const sideMenuLinks = sideMenu.querySelectorAll('a');
        sideMenuLinks.forEach(link => {
            link.addEventListener('click', function () {
                sideMenu.classList.remove('active');
                if (sideMenuOverlay) sideMenuOverlay.classList.remove('active');
                document.body.style.overflow = '';
            });
        });

        // Закрытие меню при клике на кнопку "Войти" в side-menu
        const sideMenuButton = document.getElementById('side-menu-button-click-to-hide');
        if (sideMenuButton) {
            sideMenuButton.addEventListener('click', function () {
                sideMenu.classList.remove('active');
                if (sideMenuOverlay) sideMenuOverlay.classList.remove('active');
                document.body.style.overflow = '';
                // Здесь можно добавить открытие модального окна входа
                toggleModal(); // если функция toggleModal доступна
            });
        }
    }
});
        