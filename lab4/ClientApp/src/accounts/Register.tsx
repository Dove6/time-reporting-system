import React, { useContext, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { LoginContext } from "../App";
import { Alert, Button, Form } from "react-bootstrap";
import User from "../models/User";
import ApiConnector from "../ApiConnector";

export default function Register() {
    const loginState = useContext(LoginContext);
    const navigate = useNavigate();
    const [errorMessage, setErrorMessage] = useState<string | null>(null);
    const [addedUsername, setAddedUsername] = useState<User['name']>('');

    const performRegistration = () => {
        loginState.setIsInProgress(true);
        ApiConnector.addUser({ name: addedUsername })
            .then(() => {
                loginState.setUsername(addedUsername);
                navigate('/', { replace: true });
            })
            .catch(error => setErrorMessage(error));
    };

    return (
        <>
            <h1>Rejestracja</h1>
            {errorMessage ?
            <Alert variant="danger">{errorMessage}</Alert>
            : <></>}
            <Form.Group className="mb-3">
                <Form.Label>Nazwa użytkownika</Form.Label>
                <Form.Control type="text" value={addedUsername} onChange={evt => setAddedUsername(evt.target.value)} required />
            </Form.Group>
            <Button onClick={performRegistration} variant="primary">Zarejestruj się</Button>
        </>
    );
}
