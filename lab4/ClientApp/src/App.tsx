import React, { useEffect, useState } from 'react';
import { Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import Home from './components/Home';
import Counter from './components/Counter';
import Login from './accounts/Login';
import './custom.css'
import Projects from "./projects/Projects";
import MonthlyReport from "./reports/MonthlyReport";
import fetchData from "./fetchData";

export type LoginState = {
    username: string | null;
    setUsername: (value: string | null) => void;
};

export type LastDateState = {
    lastDate: string;
    setLastDate: (value: string) => void;
};

export const LoginContext = React.createContext<LoginState>({ username: null, setUsername: () => {} });
export const LastDateContext = React.createContext<LastDateState>({ lastDate: '', setLastDate: () => {} });

export default function App() {
    const [username, setUsername] = useState<string | null>(null);
    const [lastDate, setLastDate] = useState<string>(new Date().toLocaleDateString('en-CA'));
    useEffect(() => {
        fetchData('/api/users/current')
            .then(data => setUsername(data.name));
    }, []);

    return (
        <LoginContext.Provider value={{ username: username, setUsername: setUsername }}>
            <LastDateContext.Provider value={{ lastDate: lastDate, setLastDate: setLastDate }}>
                <Routes>
                    <Route path="/" element={<Layout />}>
                        <Route index element={<Home />} />
                        <Route path="monthly" element={<MonthlyReport />} />
                        <Route path="projects" element={<Projects />} />
                        <Route path="counter" element={<Counter />} />
                        <Route path="login" element={<Login />} />
                    </Route>
                </Routes>
            </LastDateContext.Provider>
        </LoginContext.Provider>
    );
}
