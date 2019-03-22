export function generateHeader() {
    return { 'Authorization': 'Bearer ' + JSON.parse(localStorage.getItem("token")) };
}