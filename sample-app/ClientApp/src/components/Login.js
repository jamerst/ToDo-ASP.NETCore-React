import React, { Component } from 'react';

export class Login extends Component {
  constructor(props) {
    super(props);
    
    this.handleSubmit = this.handleSubmit.bind(this);
  }
  
  static displayName = Login.name;
  async handleSubmit(event) {
    event.preventDefault();

    // send form data and get response
    fetch("Auth/login", { method: "POST", body: new FormData(event.target)}).then(response => response.json()).then(jsonData => {
      // if logged in successfully
      if (jsonData["success"] === true) {
        localStorage.removeItem("token");
        localStorage.setItem("token", JSON.stringify(jsonData["token"])); // store JWT token
        this.props.history.push("/");
      } else {
        // else mark fields as invalid
        const elements = [...document.getElementsByTagName("input")];

        elements.forEach(element => {
          element.classList.add("is-invalid");
        });
      }
    });
  }
  
  render () {
    return (
      <form onSubmit={this.handleSubmit}>
        <div className="form-group">
          <label htmlFor="email">Email address</label>
          <input type="email" className="form-control" name="email" required />
          <label htmlFor="password">Password</label>
          <input type="password" className="form-control" name="password" required />
        </div>
        <button className="btn btn-primary" id="submit">Submit</button>
      </form>
    );
  }
}
