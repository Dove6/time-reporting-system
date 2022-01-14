import ProjectModel from "./ProjectModel";

type ManagedProject = ProjectModel & {
    active: boolean;
    budget: number;
    budgetLeft: number;
}

export default ManagedProject;
