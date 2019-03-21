import React, { Component } from 'react';

export class Register extends Component {
  constructor(props) {
    super(props);
    
    this.handleSubmit = this.handleSubmit.bind(this);
  }
  
  static displayName = Register.name;
  async handleSubmit(event) {
    event.preventDefault();

    // send form data and get response
    fetch("Auth/register", { method: "POST", body: new FormData(event.target)}).then(response => response.json()).then(jsonData => {
        // if success, redirect to lists page
        if (jsonData["success"] === true) {
            localStorage.setItem("token", JSON.stringify(jsonData["token"])); // store JWT token
            this.props.history.push("/");
        } else {
            alert("Error: " + jsonData["errMsg"]);
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
          <label htmlFor="confirmPassword">Confirm Password</label>
          <input type="password" className="form-control" name="confirmPassword" required />
        </div>
        <button className="btn btn-primary" id="submit">Submit</button>
      </form>
    );
  }
}
