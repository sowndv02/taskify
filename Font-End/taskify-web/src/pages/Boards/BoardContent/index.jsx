import React from 'react'
import Box from '@mui/material/Box'

function BoardContent() {
  return (
    <Box sx={{
        width: '100%',
        height: (theme) => `calc(100vh - ${theme.taskify.boardBarHeight} - ${theme.taskify.appBarHeight})`,
        backgroundColor:'primary.main',
        display: 'flex',
        alignItems: 'center'
      }}>Board Content</Box>
  )
}

export default BoardContent
