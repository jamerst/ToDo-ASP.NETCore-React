import React, { Component } from 'react';
import { Alert } from 'reactstrap';

export class Login extends Component {
  constructor(props) {
    super(props);
    
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleDismiss = this.handleDismiss.bind(this);

    this.state = { alert: false, alertMessage: "" };
  }
  
  static displayName = Login.name;

  handleSubmit(event) {
    event.preventDefault();

    // send form data and get response
    fetch("api/auth/login", { method: "POST", body: new FormData(event.target)}).then(response => response.json()).then(jsonData => {
      // if logged in successfully
      if (jsonData["success"] === true) {
        localStorage.removeItem("token");
        localStorage.setItem("token", JSON.stringify(jsonData["token"])); // store JWT token
        this.props.history.push("/");
      } else {
        // display error message
        this.setState({alert: true, alertMessage: jsonData["errMsg"]});
      }
    });
  }

  handleDismiss() {
    this.setState({alert: false});
  }
  
  render () {
    return (
      <div className="container">
        <div className="row">
          <div className="jumbotron col-12">
            <h1>Welcome to ToDo App</h1>
          </div>
        </div>
        <div className="row">
          <div className="col col-8">
            <div className="card">
              <div className="card-header">
                <h4 className="card-title">Login</h4>
              </div>
              <div className="card-body">
                <form onSubmit={this.handleSubmit}>
                  <div className="form-group">
                    <label htmlFor="email">Email address</label>
                    <input type="email" className="form-control" name="email" required />
                    <label htmlFor="password">Password</label>
                    <input type="password" className="form-control" name="password" required />
                  </div>
                  <button className="btn btn-primary" id="submit">Submit</button>
                </form>
                <Alert color="danger" className="mt-3" isOpen={this.state.alert} toggle={this.handleDismiss}>{this.state.alertMessage}</Alert>
              </div>
            </div>
          </div>
          <div className="col">
            <div className="card text-center">
              <div className="card-header">
                <h3 className="card-title">Create an account</h3>
              </div>
              <div className="card-body">
                <h4>Not got an account?</h4>
                <a className="btn btn-lg btn-primary" href="/register">Create one here</a>
              </div>
            </div>
            
          </div>
        </div>
      </div>
    );
  }
}
