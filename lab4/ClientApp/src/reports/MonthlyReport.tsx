import React, {useContext, useEffect, useState} from 'react';
import {LastDateContext} from "../App";
import MonthlyReportModel from "../models/MonthlyReport";
import fetchData from "../fetchData";
import {Table} from "react-bootstrap";

export default function MonthlyReport() {
    const lastDateState = useContext(LastDateContext);
    const getLastMonth = () => lastDateState.state.lastDate.substring(0, 7);
    const setLastMonth = (monthString: string) => lastDateState.setLastDate(`${monthString}-01`);

    const [monthlyReport, setMonthlyReport] = useState<MonthlyReportModel | null>(null);
    const refreshDailyReport = () => {
        fetchData(`/api/reports/${getLastMonth()}`)
            .then(data => setMonthlyReport(data));
    }
    useEffect(refreshDailyReport, [lastDateState.state.lastDate]);

    return (<>
        <h1>Raport czasu pracy na miesiąc {getLastMonth()}</h1>
        <Table>
            <thead>
            <tr>
                <th>Projekt</th>
                <th>Zadeklarowany czas</th>
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
