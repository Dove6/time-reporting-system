import React from 'react';
import { Outlet } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import NavMenu from './NavMenu';

export default function Layout() {
    return (
        <div>
            <NavMenu />
            <Container>
                <Outlet />
            </Container>
        </div>
    );
}
