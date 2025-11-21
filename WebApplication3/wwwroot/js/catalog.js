// catalog.js - функционал для каталога товаров и общие функции корзины
document.addEventListener('DOMContentLoaded', function () {
    initCatalog();
    if (isUserAuthenticated()) {
        loadCartCount();
    }
});

function initCatalog() {
    // Обработчики для кнопок "В корзину" в каталоге
    const addToCartButtons = document.querySelectorAll('.add-to-cart');
    addToCartButtons.forEach(button => {
        button.addEventListener('click', function () {
            const productId = this.getAttribute('data-product-id');
            addToCart(productId, 1);
        });
    });

    // Быстрый поиск при вводе
    const searchInput = document.querySelector('.search-input');
    if (searchInput) {
        let searchTimeout;
        searchInput.addEventListener('input', function () {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                if (this.value.length >= 3 || this.value.length === 0) {
                    this.closest('form').submit();
                }
            }, 500);
        });
    }

    // Анимация загрузки товаров
    observeProductCards();
}

// ★★★★ ГЛАВНАЯ ФУНКЦИЯ ДОБАВЛЕНИЯ В КОРЗИНУ ★★★★
function addToCart(productId, quantity = 1) {
    console.log('=== addToCart ===');
    console.log('productId:', productId, 'quantity:', quantity);

    if (!isUserAuthenticated()) {
        showAuthRequiredMessage();
        return;
    }

    // Показываем индикатор загрузки
    const button = document.querySelector(`[data-product-id="${productId}"]`);
    const originalText = button?.textContent || 'В корзину';

    if (button) {
        button.textContent = 'Добавляем...';
        button.disabled = true;
    }

    fetch('/Cart/AddToCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantity
        })
    })
        .then(response => {
            if (!response.ok) {
                throw new Error('Network error');
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                showNotification('Товар добавлен в корзину!', 'success');
                updateCartCounter(data.cartCount);
            } else {
                showNotification(data.error || 'Ошибка при добавлении в корзину', 'error');
            }
        })
        .catch(error => {
            console.error('Error:', error);
            showNotification('Ошибка соединения с сервером', 'error');
        })
        .finally(() => {
            if (button) {
                button.textContent = originalText;
                button.disabled = false;
            }
        });
}

// ★★★★ ОБЩИЕ ФУНКЦИИ ★★★★
function isUserAuthenticated() {
    return document.querySelector('.auth-user-info') !== null ||
        document.querySelector('#logout-btn') !== null;
}

function showAuthRequiredMessage() {
    const notification = document.createElement('div');
    notification.className = 'auth-notification';
    notification.innerHTML = `
        <div class="notification-content">
            <p>Для добавления товаров в корзину необходимо авторизоваться</p>
            <button class="button" onclick="openLoginModal()">Войти</button>
        </div>
    `;

    notification.style.cssText = `
        position: fixed;
        top: 50%;
        left: 50%;
        transform: translate(-50%, -50%);
        background: white;
        padding: 20px;
        border-radius: 8px;
        box-shadow: 0 5px 20px rgba(0,0,0,0.3);
        z-index: 10000;
        text-align: center;
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        document.addEventListener('click', function closeNotification(e) {
            if (!notification.contains(e.target)) {
                notification.remove();
                document.removeEventListener('click', closeNotification);
            }
        });
    }, 100);
}

function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `notification notification-${type}`;
    notification.textContent = message;

    notification.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: ${type === 'success' ? '#4CAF50' : type === 'error' ? '#f44336' : '#ff9800'};
        color: white;
        padding: 15px 20px;
        border-radius: 4px;
        box-shadow: 0 3px 10px rgba(0,0,0,0.2);
        z-index: 10000;
        animation: slideIn 0.3s ease;
    `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 300);
    }, 3000);
}

function updateCartCounter(count) {
    const cartCounter = document.querySelector('.cart-counter');
    if (cartCounter) {
        cartCounter.textContent = count;
        console.log('Cart counter updated to:', count);
    }
}

function loadCartCount() {
    if (!isUserAuthenticated()) return;

    fetch('/Cart/GetCartItemsCount', {
        method: 'GET'
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                updateCartCounter(data.data);
            }
        })
        .catch(error => {
            console.error('Ошибка при загрузке счетчика корзины:', error);
        });
}

function openLoginModal() {
    const loginButton = document.getElementById('click-to-hide');
    if (loginButton) {
        loginButton.click();
    }
}

function observeProductCards() {
    const productCards = document.querySelectorAll('.product-card');
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, { threshold: 0.1 });

    productCards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'opacity 0.5s ease, transform 0.5s ease';
        observer.observe(card);
    });
}

// Добавляем стили для анимаций
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from { transform: translateX(100%); opacity: 0; }
        to { transform: translateX(0); opacity: 1; }
    }
    
    @keyframes slideOut {
        from { transform: translateX(0); opacity: 1; }
        to { transform: translateX(100%); opacity: 0; }
    }
    
    .notification-content .button {
        margin-top: 10px;
    }
`;
document.head.appendChild(style);