import { createContext } from "react";

const contextUser = createContext<string | undefined>(undefined);

export default contextUser;