import { experimental_extendTheme as extendTheme} from '@mui/material/styles'
import { orange, teal, deepOrange, cyan } from '@mui/material/colors'

// Create a theme instance.
const theme = extendTheme({
    taskify:{
        appBarHeight: '48px',
        boardBarHeight: '58px'
    },
    colorSchemes: {
        light: {
            palette: {
                primary: teal,
                secondary: deepOrange
            }
        },
        dark: {
            palette: {
                primary: cyan,
                secondary: orange
            }
        }
        // spacing: (factor) => `${0.25 * factor}rem`, // (Bootstrap strategy)
    }
});

export default theme