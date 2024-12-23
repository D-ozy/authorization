document.getElementById("entranceButton").addEventListener('click', async function (event) {
    event.preventDefault();


    const userNameOrEmail = document.getElementById('userNameOrEmail').value;
    const password = document.getElementById('userPassword').value;

    const userExists = await checkUserExists(userNameOrEmail, password);

    if (userExists) {
        alert("Дароу")
        return;
    } else {
        alert("Тебя не существует")
        return;
    }
})


async function checkUserExists(userNameOrEmail, password) {
    try {
        const response = await fetch(`/entrance/get?name=${encodeURIComponent(userNameOrEmail)}&email=${encodeURIComponent(userNameOrEmail)}&password=${encodeURIComponent(password)}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        });

        if (!response.ok) {
            throw new Error('Ошибка при проверке пользователя');
        }
        const data = await response.json();

        console.log(data)

        return data.exists; // Предполагаем, что сервер возвращает { exists: true/false }
    } catch (error) {
        console.error(error);
        return false;
    }
}