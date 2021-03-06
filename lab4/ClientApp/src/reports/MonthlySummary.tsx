import React, {useContext, useEffect, useState} from 'react';
import {LastDateContext} from "../App";
import MonthlyReportModel from "../models/MonthlyReportModel";
import {Button, Table} from "react-bootstrap";
import ApiConnector from "../ApiConnector";

export default function MonthlySummary() {
    const lastDateState = useContext(LastDateContext);
    const toMonthString = (dateString: string) => dateString.substring(0, 7);
    const toDateString = (monthString: string) => `${monthString}-01`;

    const [monthlyReport, setMonthlyReport] = useState<MonthlyReportModel | null>(null);
    const setMonthlyReportFrozen = (frozen: boolean) => {
        if (!monthlyReport)
            return;
        setMonthlyReport(prevState => {
            if (prevState === null)
                return null;
            return { ...prevState, frozen: frozen };
        });
    };
    const refreshMonthlyReport = () => {
        ApiConnector.getMonthlyReport(toMonthString(lastDateState.state.lastDate))
            .then(data => setMonthlyReport(data));
    }
    useEffect(refreshMonthlyReport, [lastDateState.state.lastDate]);

    const freezeReport = () => {
        ApiConnector.freezeMonthlyReport(toMonthString(lastDateState.state.lastDate))
            .then(() => setMonthlyReportFrozen(true))
            .catch(error => alert(error));
    }

    return (<>
        <h1>Raport czasu pracy na miesiąc{' '}
            <input type="month"
                   value={toMonthString(lastDateState.state.lastDate)}
                   onChange={evt => lastDateState.setLastDate(toDateString(evt.target.value))}
            />
        </h1>
        {monthlyReport?.frozen ?
            <p className="lead">[Raport został zatwierdzony]</p> :
            <Button className="me-2" onClick={freezeReport}>Zatwierdź raport</Button>}
        <Table>
            <thead>
            <tr>
                <th>Projekt</th>
                <th>{monthlyReport?.frozen ? 'Zadeklarowany' : 'Łączny'} czas</th>
                <th>Zaakceptowany czas</th>
            </tr>
            </thead>
            <tbody>
            {monthlyReport?.projectTimeSummaries.map(e => (<tr key={e.projectCode}>
                <td>{e.projectCode}</td>
                <td>{e.time}</td>
                <td>{e.acceptedTime}</td>
            </tr>))}
            </tbody>
            <tfoot>
            <tr>
                <th>Razem</th>
                <th>{monthlyReport?.projectTimeSummaries.reduce((acc, val) => +acc + +val.time, 0)}</th>
                <th>{monthlyReport?.projectTimeSummaries.reduce((acc, val) => +acc + +(val.acceptedTime ?? 0), 0)}</th>
            </tr>
            </tfoot>
        </Table>
    </>);
}
