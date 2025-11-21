// cart.js - функционал для корзины товаров
document.addEventListener('DOMContentLoaded', function () {
    initCart();
});

function initCart() {
    // Обработчики для кнопок изменения количества
    initQuantityHandlers();

    // Обработчики для удаления товаров
    initRemoveHandlers();

    // Обработчики для очистки корзины и оформления заказа
    initCartActions();
}

function initQuantityHandlers() {
    // Кнопки "+"
    document.querySelectorAll('.plus-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const productId = this.getAttribute('data-product-id');
            const input = document.querySelector(`.quantity-input[data-product-id="${productId}"]`);
            const newValue = parseInt(input.value) + 1;
            input.value = newValue;
            updateQuantity(productId, newValue);
        });
    });

    // Кнопки "-"
    document.querySelectorAll('.minus-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const productId = this.getAttribute('data-product-id');
            const input = document.querySelector(`.quantity-input[data-product-id="${productId}"]`);
            const newValue = Math.max(1, parseInt(input.value) - 1);
            input.value = newValue;
            updateQuantity(productId, newValue);
        });
    });

    // Ручной ввод количества
    document.querySelectorAll('.quantity-input').forEach(input => {
        input.addEventListener('change', function () {
            const productId = this.getAttribute('data-product-id');
            const newValue = Math.max(1, parseInt(this.value) || 1);
            this.value = newValue;
            updateQuantity(productId, newValue);
        });
    });
}

function initRemoveHandlers() {
    document.querySelectorAll('.remove-btn').forEach(btn => {
        btn.addEventListener('click', function () {
            const productId = this.getAttribute('data-product-id');
            removeFromCart(productId);
        });
    });
}

function initCartActions() {
    // Очистка корзины
    const clearCartBtn = document.querySelector('.clear-cart-btn');
    if (clearCartBtn) {
        clearCartBtn.addEventListener('click', function () {
            if (confirm('Вы уверены, что хотите очистить корзину?')) {
                clearCart();
            }
        });
    }

    // Оформление заказа
    const checkoutBtn = document.querySelector('.checkout-btn');
    if (checkoutBtn) {
        checkoutBtn.addEventListener('click', function () {
            checkout();
        });
    }
}

function updateQuantity(productId, quantity) {
    showLoading(`Обновление количества...`);

    fetch('/Cart/UpdateQuantity', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            productId: productId,
            quantity: quantity
        })
    })
        .then(response => response.json())
        .then(data => {
            hideLoading();
            if (data.success) {
                showNotification(data.message, 'success');
                // Обновляем отображение на странице
                updateCartDisplay();
            } else {
                showNotification(data.error, 'error');
            }
        })
        .catch(error => {
            hideLoading();
            showNotification('Ошибка при обновлении количества', 'error');
            console.error('Error:', error);
        });
}

function removeFromCart(productId) {
    if (!confirm('Удалить товар из корзины?')) {
        return;
    }

    showLoading(`Удаление товара...`);

    fetch('/Cart/RemoveFromCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({
            productId: productId
        })
    })
        .then(response => response.json())
        .then(data => {
            hideLoading();
            if (data.success) {
                showNotification(data.message, 'success');
                // Удаляем элемент из DOM
                const cartItem = document.querySelector(`.cart-item[data-product-id="${productId}"]`);
                if (cartItem) {
                    cartItem.remove();
                }
                // Обновляем счетчик в шапке
                updateHeaderCartCount(data.cartCount);
                // Обновляем отображение корзины
                updateCartDisplay();
            } else {
                showNotification(data.error, 'error');
            }
        })
        .catch(error => {
            hideLoading();
            showNotification('Ошибка при удалении товара', 'error');
            console.error('Error:', error);
        });
}

function clearCart() {
    showLoading(`Очистка корзины...`);

    fetch('/Cart/ClearCart', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        }
    })
        .then(response => response.json())
        .then(data => {
            hideLoading();
            if (data.success) {
                showNotification(data.message, 'success');
                // Очищаем корзину в DOM
                const cartItems = document.querySelector('.cart-items');
                if (cartItems) {
                    cartItems.innerHTML = '';
                }
                // Обновляем счетчик в шапке
                updateHeaderCartCount(0);
                // Показываем сообщение о пустой корзине
                showEmptyCart();
            } else {
                showNotification(data.error, 'error');
            }
        })
        .catch(error => {
            hideLoading();
            showNotification('Ошибка при очистке корзины', 'error');
            console.error('Error:', error);
        });
}

function checkout() {
    showNotification('Функция оформления заказа в разработке', 'info');
    // Здесь будет логика оформления заказа
}

function updateCartDisplay() {
    // Пересчитываем общую сумму
    let totalAmount = 0;
    let totalItems = 0;

    document.querySelectorAll('.cart-item').forEach(item => {
        const productId = item.getAttribute('data-product-id');
        const quantity = parseInt(document.querySelector(`.quantity-input[data-product-id="${productId}"]`).value);
        const price = parseFloat(item.querySelector('.cart-item-price').textContent.replace(' руб.', '').replace(' ', ''));
        const total = quantity * price;

        // Обновляем отображение общей стоимости для товара
        item.querySelector('.total-price').textContent = total.toFixed(2) + ' руб.';

        totalAmount += total;
        totalItems += quantity;
    });

    // Обновляем итоговую сумму
    const totalAmountElement = document.querySelector('.total-amount');
    const totalItemsElement = document.querySelector('.summary-row span:last-child');

    if (totalAmountElement) {
        totalAmountElement.textContent = totalAmount.toFixed(2) + ' руб.';
    }

    if (totalItemsElement) {
        totalItemsElement.textContent = totalItems + ' шт.';
    }

    // Если товаров нет, показываем сообщение о пустой корзине
    if (totalItems === 0) {
        showEmptyCart();
    }
}

function showEmptyCart() {
    const cartContainer = document.querySelector('.cart-container');
    if (cartContainer) {
        cartContainer.innerHTML = `
            <div class="empty-cart">
                <h3>Ваша корзина пуста</h3>
                <p>Добавьте товары из каталога, чтобы они появились здесь</p>
                <a href="/Product/Catalog" class="button">Перейти к товарам</a>
            </div>
        `;
    }
}

function updateHeaderCartCount(count) {
    const cartCounter = document.querySelector('.cart-counter');
    if (cartCounter) {
        cartCounter.textContent = count;
    }
}

// Вспомогательные функции
function showLoading(message = 'Загрузка...') {
    // Можно добавить индикатор загрузки
    console.log(message);
}

function hideLoading() {
    // Скрыть индикатор загрузки
}
function updateHeaderCartCount(count) {
    const cartCounter = document.querySelector('.cart-counter');
    if (cartCounter) {
        cartCounter.textContent = count;
    }
}

/*function showNotification(message, type = 'info') {
    // Создаем уведомление
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

    // Автоматическое закрытие через 3 секунды
    setTimeout(() => {
        notification.style.animation = 'slideOut 0.3s ease';
        setTimeout(() => {
            if (notification.parentNode) {
                notification.remove();
            }
        }, 300);
    }, 3000);
}*/
