import React, { Component } from 'react';
import { Alert } from 'reactstrap';
import { generateHeader } from '../util.js';

export class Home extends Component {
    constructor(props) {
        super(props);
        
        this.logout = this.logout.bind(this);
        this.handleAlertDismiss = this.handleAlertDismiss.bind(this);
        this.fetchLists = this.fetchLists.bind(this);

        this.handleToggle = this.handleToggle.bind(this);
        this.handleListCreate = this.handleListCreate.bind(this);
        this.handleListDelete = this.handleListDelete.bind(this);
        this.handleItemCreate = this.handleItemCreate.bind(this);
        this.handleItemDelete = this.handleItemDelete.bind(this);

        this.state = { lists: [],
            loading: true,
            alert: false,
            alertMessage: ""
         };
        this.fetchLists();
    }

    static displayName = Home.name;

    logout() {
        localStorage.removeItem("token");
        this.props.history.push("/login")
    }

    handleAlertDismiss() {
        this.setState({ alert: false });
    }

    fetchLists() {
        fetch("List/getLists", { headers: generateHeader() })
        .then(response => {
            // if access denied, redirect to login
            if (response.status === 401) {
                this.logout();
            } else {
                return response.json();
            }
        }).then(jsonData => {
            if (jsonData["success"]) {
                this.setState({ lists: jsonData["lists"], loading: false });
            } else if (jsonData["code"] === 401) { // if access denied due to invalid UID, redirect to login
                this.logout();
            }
        });
    }

    handleToggle(event) {
        let data = new FormData();
        data.append("itemId", event.target.value);
        data.append("complete", event.target.checked);
        fetch("List/updateItem", { method: "POST", headers: generateHeader(), body: data })
            .then(response => response.json()).then(jsonData => {
                if (jsonData["success"]) {
                    this.fetchLists(); // update state
                } else {
                    this.setState({alert: true, alertMessage: jsonData["errMsg"]});
                }
        })
    }

    handleListCreate() {
        let name = prompt("Please enter a list name:");
        if (name != null) {
            let data = new FormData();
            data.append("listName", name);
            fetch("List/createList", { method: "POST", headers: generateHeader(), body: data })
                .then(response => response.json()).then(jsonData => {
                    if (jsonData["success"] === true) {
                        this.fetchLists(); // update state
                    } else {
                        this.setState({ alert: true, alertMessage: jsonData["errMsg"] });
                    }
                })
        }
    }

    handleListDelete(event) {
        if (window.confirm("Are you sure you want to delete this list?")) {
            let data = new FormData();
            data.append("listId", event.target.value);
            fetch("List/deleteList", { method: "POST", headers: generateHeader(), body: data })
                .then(response => response.json()).then(jsonData => {
                    if (jsonData["success"] === true) {
                        this.fetchLists(); // update state
                    } else {
                        this.setState({ alert: true, alertMessage: jsonData["errMsg"] });                        
                    }
                })
        }
    }

    handleItemCreate(event) {
        let text = prompt("Please enter item text:");
        if (text != null) {
            let data = new FormData();
            data.append("listId", event.target.value);
            data.append("text", text);
            fetch("List/createItem", { method: "POST", headers: generateHeader(), body: data })
                .then(response => response.json()).then(jsonData => {
                    if (jsonData["success"] === true) {
                        this.fetchLists(); // update state
                    } else {
                        this.setState({ alert: true, alertMessage: jsonData["errMsg"] });                        
                    }
                })
        }
    }

    handleItemDelete(event) {
        if (window.confirm("Are you sure you want to remove this item?")) {
            let data = new FormData();
            data.append("itemId", event.target.value);
            fetch("List/deleteItem", { method: "POST", headers: generateHeader(), body: data })
                .then(response => response.json()).then(jsonData => {
                    if (jsonData["success"] === true) {
                        this.fetchLists(); // update state
                    } else {
                        this.setState({ alert: true, alertMessage: jsonData["errMsg"] });                        
                    }
                })
        }
    }

    renderLists(lists) {
        return (
            <React.Fragment>
            <div className="container mb-3">
                <div className="d-flex justify-content-end">
                    <Alert color="danger" className="mt-3" isOpen={this.state.alert} toggle={this.handleAlertDismiss}>{this.state.alertMessage}</Alert>
                    <button className="btn btn-primary" onClick={this.handleListCreate}>Create list</button>
                </div>
            </div>
            {lists.map(list =>
                <div className="card mb-3" key={list.id}>
                    <div className="card-header d-flex justify-content-between"><h3>{list.name}</h3><button className="btn btn-danger" value={list.id} onClick={this.handleListDelete}>Delete List</button></div>
                    <div className="card-body">
                        <ul className="list-group list-group-flush">
                            {list.items.map(item =>
                                <li className="list-group-item d-flex justify-content-between" key={item.id}>
                                    <div>
                                        <input type="checkbox" className="form-check-input" value={item.id} id={item.id} defaultChecked={item.complete} onChange={this.handleToggle} />
                                        <label htmlFor={item.id} className="form-check-label col">{item.text}</label>
                                    </div>
                                    <button className="btn btn-sm btn-outline-danger" value={item.id} onClick={this.handleItemDelete}>Remove</button>
                                </li>
                            )}
                            <li className="list-group-item d-flex justify-content-end"><button className="btn btn-sm btn-primary" value={list.id} onClick={this.handleItemCreate}>Add item</button></li>
                        </ul>
                    </div>
                </div>
            )}
            </React.Fragment>
        )
    }

    render () {
        if (this.state.loading) {
            return "Loading lists.."
        }
        
        return this.renderLists(this.state.lists);
    }
}
