import React, { Component } from 'react';
import { Alert } from 'reactstrap';

export class Register extends Component {
  constructor(props) {
    super(props);
    
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleDismiss = this.handleDismiss.bind(this);

    this.state = { alert: false, alertMessage: "" };
  }
  
  static displayName = Register.name;
  async handleSubmit(event) {
    event.preventDefault();

    // send form data and get response
    fetch("api/auth/register", { method: "POST", body: new FormData(event.target)}).then(response => response.json()).then(jsonData => {
        // if success, redirect to lists page
        if (jsonData["success"] === true) {
            localStorage.setItem("token", JSON.stringify(jsonData["token"])); // store JWT token
            this.props.history.push("/");
        } else {
            this.setState({alert: true, alertMessage: jsonData["errMsg"]});
        }
    });  
  }

  handleDismiss() {
    this.setState({ alert: false });
  }
  
  render () {
    return (
      <div className="container">
        <div className="row">
          <div className="jumbotron col-12 text-center">
            <h1>Registration</h1>
          </div>
        </div>
      <div className="row justify-content-center">
          <div className="card col col-8">
            <div className="card-body">
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
              <Alert color="danger" className="mt-3" isOpen={this.state.alert} toggle={this.handleDismiss}>{this.state.alertMessage}</Alert>
            </div>
          </div>
        </div>
      </div>
    );
  }
}
