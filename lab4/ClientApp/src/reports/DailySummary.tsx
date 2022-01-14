import ProjectModel from "../models/ProjectModel";
import ReportEntryModel from "../models/ReportEntryModel";
import {Table} from "react-bootstrap";
import '../custom.css';

type DailySummaryProps = {
    entries: ReportEntryModel[];
    projects: ProjectModel[];
};

type DictStrNum = { [key: string]: number };
type DictStrStr = { [key: string]: string };

export default function DailySummary(props: DailySummaryProps) {
    const projectTimeGrouping = Object.values(props.entries).reduce((acc: DictStrNum, val) =>
        ({ ...acc, [val.projectCode]: ((acc[val.projectCode] ?? 0) + val.time) }), {});
    const projectCodeNameMap = props.projects.reduce((acc: DictStrStr, val) =>
        ({ ...acc, [val.code]: val.name }), {});

    return (<Table>
        <thead>
            <tr>
                <th>Projekt</th>
                <th>Łączny czas</th>
            </tr>
        </thead>
        <tbody>
            {Object.keys(projectTimeGrouping).sort().map(e => (<tr key={e}>
                <td>{projectCodeNameMap[e]} ({e})</td>
                <td>{projectTimeGrouping[e]}</td>
            </tr>))}
        </tbody>
        <tfoot>
            <tr>
                <th>Razem</th>
                <th>{Object.values(projectTimeGrouping).reduce((acc, val) => +acc + +val, 0)}</th>
            </tr>
        </tfoot>
    </Table>);
}
