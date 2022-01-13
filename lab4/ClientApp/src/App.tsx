import React, { useEffect, useState } from 'react';
import { Route, Routes } from 'react-router-dom';
import Layout from './layout/Layout';
import DailyReport from './reports/DailyReport';
import Login from './accounts/Login';
import './custom.css'
import MonthlySummary from "./reports/MonthlySummary";
import fetchData from "./fetchData";
import EnsureLoggedIn from "./accounts/EnsureLoggedIn";

export type LoginState = {
    isInProgress: boolean;
    username: string | null;
}
export type LoginStateInContext = {
    state: LoginState;
    setState: Function;
    setIsInProgress: (isInProgress: boolean) => void;
    setUsername: (username: string | null) => void;
};

export type LastDateState = {
    lastDate: string;
}
export type LastDateStateInContext = {
    state: LastDateState;
    setState: Function;
    setLastDate: (lastDate: string) => void;
};

export const LoginContext = React.createContext<LoginStateInContext>({
    state: { isInProgress: true, username: null },
    setState: () => {},
    setIsInProgress: () => {},
    setUsername: () => {},
});
export const LastDateContext = React.createContext<LastDateStateInContext>({
    state: { lastDate: '' },
    setState: () => {},
    setLastDate: () => {}
});

export default function App() {
    const [loginState, setLoginState] = useState<LoginState>({
        isInProgress: true,
        username: null
    });
    const [lastDateState, setLastDateState] = useState<LastDateState>({
        lastDate: new Date().toLocaleDateString('en-CA')
    });

    const setLoginStateUsername = (username: string | null) => {
        setLoginState(prevState => ({...prevState, username: username }))
    }
    const setLoginStateIsInProgress = (isInProgress: boolean) => {
        setLoginState(prevState => ({...prevState, isInProgress: isInProgress }))
    }
    const setLastDate = (lastDate: string) => {
        setLastDateState(prevState => ({...prevState, lastDate: lastDate }))
    }

    useEffect(() => {
        fetchData('/api/users/current')
            .then(data => setLoginStateUsername(data.name))
            .finally(() => setLoginStateIsInProgress(false));
    }, []);

    const loginProvider = {
        state: loginState,
        setState: setLoginState,
        setIsInProgress: setLoginStateIsInProgress,
        setUsername: setLoginStateUsername
    };
    const lastDateProvider = {
        state: lastDateState,
        setState: setLastDateState,
        setLastDate: setLastDate
    };

    return (
        <LoginContext.Provider value={loginProvider}>
            <LastDateContext.Provider value={lastDateProvider}>
                <Routes>
                    <Route path="/" element={<Layout />}>
                        <Route index element={
                            <EnsureLoggedIn>
                                <DailyReport />
                            </EnsureLoggedIn>
                        } />
                        <Route path="monthly" element={
                            <EnsureLoggedIn>
                                <MonthlySummary />
                            </EnsureLoggedIn>
                        } />
                        <Route path="login" element={<Login />} />
                    </Route>
                </Routes>
            </LastDateContext.Provider>
        </LoginContext.Provider>
    );
}
