import CategoryModel from './CategoryModel';

type ProjectCreationRequest = {
    budget: number;
    name: string;
    categories: CategoryModel[];
}

export default ProjectCreationRequest;
