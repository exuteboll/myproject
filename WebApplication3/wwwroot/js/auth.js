// auth.js - Скрипты для работы с авторизацией

// Проверка статуса авторизации
function checkAuthStatus() {
    fetch('/Home/CheckAuth')
        .then(response => response.json())
        .then(data => {
            if (data.isAuthenticated) {
                updateUIForAuthenticatedUser(data.user);
            } else {
                updateUIForGuest();
            }
        })
        .catch(error => {
            console.error('Ошибка при проверке авторизации:', error);
        });
}

// Обновление UI для авторизованного пользователя
function updateUIForAuthenticatedUser(user) {
    // Скрываем модальное окно входа если открыто
    const modal = document.querySelector(".container-login-registration");
    if (modal) {
        modal.style.display = "none";
    }

    console.log('Пользователь авторизован:', user);
}

// Обновление UI для гостя
function updateUIForGuest() {
    console.log('Пользователь не авторизован');
}

// Выход из системы
function logout() {
    fetch('/Home/Logout', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                location.reload();
            } else {
                console.error('Ошибка при выходе:', data.error);
                alert('Ошибка при выходе из системы');
            }
        })
        .catch(error => {
            console.error('Ошибка при выходе:', error);
            alert('Ошибка при выходе из системы');
        });
}

// Инициализация обработчиков событий для авторизации
function initAuthHandlers() {
    // Обработчик выхода для десктопной версии
    const logoutBtn = document.getElementById('logout-btn');
    if (logoutBtn) {
        logoutBtn.addEventListener('click', function (e) {
            e.preventDefault();
            logout();
        });
    }

    // Обработчик выхода для мобильной версии
    const sideMenuLogoutBtn = document.getElementById('side-menu-logout-btn');
    if (sideMenuLogoutBtn) {
        sideMenuLogoutBtn.addEventListener('click', function (e) {
            e.preventDefault();
            logout();

            // Закрываем боковое меню после выхода
            const sideMenu = document.getElementById('side-menu');
            const sideMenuOverlay = document.getElementById('side-menu-overlay');
            if (sideMenu) sideMenu.classList.remove('active');
            if (sideMenuOverlay) sideMenuOverlay.classList.remove('active');
        });
    }

    // Проверяем авторизацию при загрузке страницы
    document.addEventListener('DOMContentLoaded', function () {
        checkAuthStatus();
    });
}

// Запуск инициализации при загрузке скрипта
initAuthHandlers();