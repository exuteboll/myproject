// product-details.js - функционал для детальной страницы товара
document.addEventListener('DOMContentLoaded', function () {
    initProductDetails();
});

function initProductDetails() {
    // Обработчик для кнопки "В корзину" на странице деталей
    const addToCartButton = document.querySelector('.add-to-cart-large');
    if (addToCartButton) {
        addToCartButton.addEventListener('click', function () {
            const productId = this.getAttribute('data-product-id');
            const quantityInput = document.getElementById('quantity');
            const quantity = quantityInput ? parseInt(quantityInput.value) : 1;

            // Используем общую функцию из catalog.js
            addToCart(productId, quantity);
        });
    }

    // Валидация количества
    const quantityInput = document.getElementById('quantity');
    if (quantityInput) {
        quantityInput.addEventListener('change', function () {
            if (this.value < 1) {
                this.value = 1;
            }
            if (this.value > 999) {
                this.value = 999;
            }
        });
    }

    // Переключение изображений (если есть галерея)
    initImageGallery();
}

function initImageGallery() {
    const mainImage = document.getElementById('main-product-image');
    const thumbnails = document.querySelectorAll('.thumbnail');

    if (!mainImage || !thumbnails.length) return;

    thumbnails.forEach(thumbnail => {
        thumbnail.addEventListener('click', function () {
            const newImageSrc = this.getAttribute('data-image');

            // Плавная смена изображения
            mainImage.style.opacity = '0';

            setTimeout(() => {
                mainImage.src = newImageSrc;
                mainImage.style.opacity = '1';
            }, 200);

            // Подсветка активной миниатюры
            thumbnails.forEach(t => t.style.border = '2px solid transparent');
            this.style.border = '2px solid rgba(255, 165, 0, 1)';
        });
    });
}