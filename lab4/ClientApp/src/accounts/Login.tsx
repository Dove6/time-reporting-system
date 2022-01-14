import React, { useContext, useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { LoginContext } from "../App";
import { Alert, Button, Form } from "react-bootstrap";
import User from "../models/User";
import ApiConnector from "../ApiConnector";

export default function Login() {
    const loginState = useContext(LoginContext);
    const navigate = useNavigate();
    const [selectedName, setSelectedName] = useState('');
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [users, setUsers] = useState<User[]>([]);
    useEffect(() => {
        ApiConnector.getUsers()
            .then(data => setUsers(data));
    }, []);

    const performLogin = () => {
        loginState.setIsInProgress(true);
        ApiConnector.login({ name: selectedName })
            .then(() => {
                loginState.setUsername(selectedName);
                navigate('/', { replace: true });
            })
            .catch(error => setErrorMessage(error));
    };

    return (
        <>
            <h1>Logowanie</h1>
            {errorMessage ?
            <Alert variant="danger">{errorMessage}</Alert>
            : <></>}
            <Form.Group className="mb-3">
                <Form.Label>Nazwa użytkownika</Form.Label>
                <Form.Select value={selectedName} onChange={evt => setSelectedName(evt.target.value)} required>
                    <option value="" disabled hidden>Wybierz...</option>
                    {users.map(user => (
                        <option key={user.name} value={user.name}>{user.name}</option>
                    ))}
                </Form.Select>
            </Form.Group>
            <Button onClick={performLogin} variant="primary">Zaloguj się</Button>
            <hr />
            <p>
                Nie posiadasz konta?{' '}
                <Link to="/register">Zarejestruj się!</Link>
            </p>
        </>
    );
}
