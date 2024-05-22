import React from 'react'

function BoardBar() {
  return (
    <Box sx={{
        backgroundColor: 'primary.dark',
        width: '100%',
        height: (theme) => theme.taskify.boardBarHeight,
        display: 'flex',
        alignItems: 'center'
      }}>Board Bar</Box>
  )
}

export default BoardBar
