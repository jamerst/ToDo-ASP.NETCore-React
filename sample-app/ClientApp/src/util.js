export function generateHeader() {
    // return { "Authorization": "Bearer " + JSON.parse(localStorage.getItem("token")) }
    return { 'Authorization': 'Bearer ' + JSON.parse(localStorage.getItem("token")) };
}

export function logout() {
    localStorage.removeItem("token");
}