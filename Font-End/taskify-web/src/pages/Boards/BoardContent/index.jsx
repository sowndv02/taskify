import React from 'react'

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
