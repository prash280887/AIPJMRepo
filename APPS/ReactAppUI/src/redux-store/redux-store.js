import { configureStore } from '@reduxjs/toolkit';  
import homeSliceReducer from './redux-homeSlice';

const reduxStore = configureStore({
    reducer: {
        // Add your slices here
        homeSlice : homeSliceReducer 
        // you can add more slices (consumed for each component) as needed   
        },

    });

export default reduxStore;