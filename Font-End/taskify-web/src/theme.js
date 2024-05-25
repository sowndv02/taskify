import { experimental_extendTheme as extendTheme} from '@mui/material/styles'
import { orange, teal, deepOrange, cyan } from '@mui/material/colors'

// Create a theme instance.
const theme = extendTheme({
    taskify:{
        appBarHeight: '58px',
        boardBarHeight: '60px'
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
    },
    components: {
        MuiCssBaseline: { 
            styleOverrides: {
                body: {
                    '*::-webkit-scrollbar':{
                        window: '8px',
                        height: '8px'
                    },
                    '*::-webkit-scrollbar-thumb':{
                        backgroundColor: '#bdc3b7',
                        borderRadius: '8px'
                    },
                    '*::-webkit-scrollbar-thumb::hover':{
                        backgroundColor: '#00b894',
                    }
                }
            }
        },
        MuiButton: {
            styleOverrides: {
                root: {
                    textTransform: 'none'
                }
            }
        },
        MuiOutlinedInput: {
            styleOverrides: {
                root: ({ theme }) => ({
                    color: theme.palette.primary.main,
                    fontSize: '0.875rem',
                    '.MuiOutlinedInput-notchedOutline': {
                        borderColor: theme.palette.primary.light
                    },
                    '&:hover':{
                        '.MuiOutlinedInput-notchedOutline': {
                            borderColor: theme.palette.primary.main
                        }
                    },
                    '& fieldset': {
                        borderWidth: '1px !important'
                    }
                })
            }
        },
        MuiInputLabel: {
            styleOverrides: {
                root: ({ theme }) => ({
                    color: theme.palette.primary.main,
                    fontSize: '0.875rem'
                })
            }
        }
    }
    
});

export default theme