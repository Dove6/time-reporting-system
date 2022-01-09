import React, { useContext } from 'react';
import { Container, Nav, Navbar } from 'react-bootstrap';
import { Link } from 'react-router-dom';
import './NavMenu.css';
import { LoginContext } from "../App";

export default function NavMenu() {
    const loginState = useContext(LoginContext);

    const performLogout = () => {
        fetch(`/api/users/logout`, {
            method: 'POST'
        })
            .finally(() => loginState.setUsername(null));
    };

    const loggedOutPanel = (
        <Nav.Item>
            <Nav.Link as={Link} className="text-dark" to="/login">Zaloguj się</Nav.Link>
        </Nav.Item>
    );
    const loggedInPanel = (
        <>
            <Nav.Item className="nav-link text-dark">
                Zalogowany jako {loginState.username}
            </Nav.Item>
            <Nav.Item>
                <Nav.Link className="text-dark" onClick={performLogout}>Wyloguj się</Nav.Link>
            </Nav.Item>
        </>
    );

    return (
        <header>
            <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" bg="light">
                <Container>
                    <Navbar.Brand as={Link} to="/">Trs</Navbar.Brand>
                    <Navbar.Toggle className="mr-2" />
                    <Navbar.Collapse className="d-sm-inline-flex flex-sm-row">
                        <ul className="navbar-nav flex-grow-1">
                            <Nav.Item>
                                <Nav.Link as={Link} className="text-dark" to="/">Strona główna</Nav.Link>
                            </Nav.Item>
                            {loginState.username !== null ? <>
                                <Nav.Item>
                                    <Nav.Link as={Link} className="text-dark" to="/monthly">Zestawienie miesięczne</Nav.Link>
                                </Nav.Item>
                                <Nav.Item>
                                    <Nav.Link as={Link} className="text-dark" to="/projects">Zarządzane projekty</Nav.Link>
                                </Nav.Item>
                            </> : <></>}
                            <Nav.Item className="spacer flex-grow-1"> </Nav.Item>
                            {loginState.username !== null ? loggedInPanel : loggedOutPanel}
                        </ul>
                    </Navbar.Collapse>
                </Container>
            </Navbar>
        </header>
    );
}
