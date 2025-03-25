import { useState, useEffect } from "react";
import { Form, Col, Row, Button, Alert, Nav } from "react-bootstrap";
import axios from "axios";

const StartPage = ({ onLoginSuccess }) => {
  const [isRegister, setIsRegister] = useState(true);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(true); // New state for loading
  const API_BASE_URL = "http://localhost:7027/api/account";

  useEffect(() => {
    const token = localStorage.getItem("token");
    debugger
    if (token) {
      autoLogin(token);
    } else {
      setLoading(false); // Stop loading if no token is found
    }
  }, []);

  const autoLogin = async (token) => {
    try {
      const response = await axios.get(`${API_BASE_URL}/checkAuth`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      const userName = response.data?.userName;
      const userId = response.data?.userId;
      debugger
      if (userName && userId) {
        onLoginSuccess(userName, userId);
      } else {
        throw new Error("Failed to retrieve user details.");
      }
    } catch (err) {
      console.error("Auto-login failed:", err);
      localStorage.removeItem("token"); // Clear invalid token
    } finally {
      setLoading(false); // Stop loading regardless of success or failure
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    try {
      if (isRegister) {
        await axios.post(`${API_BASE_URL}/register`, { userName: name, email, password });
        alert('User registered successfully');
        setName('');
        setEmail('');
        setPassword('');
        setIsRegister(false); // Switch to login
      } else {
        const response = await axios.post(`${API_BASE_URL}/login`, { email, password });
        const userName = response.data?.user?.userName;
        const userId = response.data?.user?.id;
        const token = response.data?.token;

        localStorage.setItem("token", token);

        if (!userName) {
          throw new Error("Failed to retrieve user name from server response.");
        }

        alert(`Welcome ${userName}`);
        onLoginSuccess(userName, userId);
      }
    } catch (err) {
      const errorMessage = err.response?.data?.message || err.response?.data?.detail || "An error occurred";
      setError(errorMessage);
    }
  };

  if (loading) {
    return <div>Loading...</div>; // Show a loading indicator while checking for token
  }

  return (
    <div className="start-page-container px-5 py-5">
      <Nav variant="pills" className="justify-content-center mb-4">
        <Nav.Item>
          <Nav.Link active={isRegister} onClick={() => setIsRegister(true)}>Register</Nav.Link>
        </Nav.Item>
        <Nav.Item>
          <Nav.Link active={!isRegister} onClick={() => setIsRegister(false)}>Login</Nav.Link>
        </Nav.Item>
      </Nav>

      {isRegister ? (
        <Form onSubmit={handleSubmit}>
          <h3 className="text-center">Create an Account</h3>
          <Form.Group className="mb-3">
            <Form.Control type="text" placeholder="Name" value={name} onChange={(e) => setName(e.target.value)} required />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Control type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Control type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} required />
          </Form.Group>
          {error && <Alert variant="danger">{error}</Alert>}
          <Button type="submit" variant="primary" className="w-100">Register</Button>
        </Form>
      ) : (
        <Form onSubmit={handleSubmit}>
          <h3 className="text-center">Login</h3>
          <Form.Group className="mb-3">
            <Form.Control type="email" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </Form.Group>
          <Form.Group className="mb-3">
            <Form.Control type="password" placeholder="Password" value={password} onChange={(e) => setPassword(e.target.value)} required />
          </Form.Group>
          {error && <Alert variant="danger">{error}</Alert>}
          <Button type="submit" variant="success" className="w-100">Login</Button>
        </Form>
      )}
    </div>
  );
};

export default StartPage;
