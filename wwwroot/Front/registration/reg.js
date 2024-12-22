document.getElementById('registerButton').addEventListener('click', async function (event) {
    event.preventDefault();

    const name = document.getElementById('name').value;
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;

    // Проверяем, существует ли пользователь
    const userExists = await checkUserExists(name);
    if (userExists) {
        document.getElementById('message').innerText = 'Пользователь с таким именем уже существует.';
        return; // Завершаем выполнение, если пользователь существует
    }

    // Если пользователь не существует, создаем нового
    const user = { name, email, password };

    try {
        const response = await fetch('/auth/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(user)
        });

        if (response.ok) {
            const result = await response.json();
            document.getElementById('message').innerText = `Пользователь ${name} успешно зарегистрирован!`;
        } else {
            const error = await response.json();
            document.getElementById('message').innerText = `Ошибка: ${error.message}`;
        }
    } catch (error) {
        document.getElementById('message').innerText = `Ошибка: ${error.message}`;
    }
});

async function checkUserExists(name) {
    try {
        const response = await fetch(`/auth/check?name=${encodeURIComponent(name)}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });
        if (!response.ok) {
            throw new Error('Ошибка при проверке пользователя');
        }
        const data = await response.json();
        return data.exists; // Предполагаем, что сервер возвращает { exists: true/false }
    } catch (error) {
        console.error(error);
        return false;
    }
}