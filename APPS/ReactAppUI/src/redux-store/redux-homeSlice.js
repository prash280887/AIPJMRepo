import { createSlice } from '@reduxjs/toolkit';


//this is slice  for Homepage
const homeSlice = createSlice({
    name: 'home',
    initialState: { status : 'idle' , isAutheticated : false}, //this is shared DATA object accessed across components using useSelector
    reducers: {
        setStatus: (state, action) => {  //this is shared METHOD to set shared DATA object across components using useDispatch
            state.status = action.payload;  
        },
        setIsAuthenticated: (state, action) => {
            state.isAutheticated = action.payload;
        },

    },
});

    export const {setStatus, setIsAuthenticated} = homeSlice.actions;
    export default homeSlice.reducer;

    //2. CreateStore to combine all above slices in redux-store.js
    //3. Add redux Provider in App.tsx & pass redux store to all components

//conventional redux reducer function (not using createSlice)
//import { useReducer } from 'react-redux';

// const reducer = (state,action) =>{

//      switch(action.type){
//         case 'setStatus':
//             return {...state, status: action.payload};
//         default:
//             return state;
//      }
// }

// export default reducer;