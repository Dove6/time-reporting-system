import Category from './Category';

type ProjectCreationRequest = {
    budget: number;
    name: string;
    categories: Category[];
}

export default ProjectCreationRequest;
