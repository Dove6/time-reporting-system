import Project from "./Project";

type ManagedProject = Project & {
    active: boolean;
    budget: number;
    budgetLeft: number;
    timestamp: string;
}

export default ManagedProject;
