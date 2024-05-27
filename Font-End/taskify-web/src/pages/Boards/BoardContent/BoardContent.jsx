import Box from '@mui/material/Box'
import ListColumns from './ListColumns/ListColumns'
import {mapOrder} from '~/utils/sorts'
import {
  DndContext,
  PointerSensor,
  MouseSensor,
  TouchSensor,
  useSensor,
  useSensors,
  DragOverlay,
  defaultDropAnimationSideEffects
} from '@dnd-kit/core';
import { useEffect, useState } from 'react';
import { arrayMove } from '@dnd-kit/sortable';
import Column from './ListColumns/Column/Column'
import Card from './ListColumns/Column/ListCards/Card/Card'

const ACTIVE_DRAG_ITEM_TYPE = {
  COLUMN: 'ACTIVE_DRAG_ITEM_TYPE_COLUMN',
  CARD:  'ACTIVE_DRAG_ITEM_TYPE_CARD'
}

function BoardContent({ board }) {
  const pointerSensor = useSensor(PointerSensor, { activationConstraint: { distance: 10 } })
  const mouseSensor = useSensor(MouseSensor, { activationConstraint: { distance: 10 } })
  const touchSensor = useSensor(TouchSensor, { activationConstraint: { 
    delay: 250,
    tolerance: 500
   }})

  // const sensors = useSensors(pointerSensor)
  const sensors = useSensors(mouseSensor, touchSensor)

  const [orderdColumns, setOrderdColumns] = useState([])

  const [activeDragItemId, setActiveDragItemId] = useState(null)
  const [activeDragItemType, setActiveDragItemType] = useState(null)
  const [activeDragItemData, setActiveDragItemData] = useState(null)




  useEffect(() => {
    setOrderdColumns(mapOrder(board?.columns, board?.columnOrderIds, '_id'))
  }, [board])

  const handleDragStart = (event) =>{
    setActiveDragItemId(event?.active?.id)
    setActiveDragItemType(event?.active?.data?.current?.columnId ? ACTIVE_DRAG_ITEM_TYPE.CARD : ACTIVE_DRAG_ITEM_TYPE.COLUMN)
    setActiveDragItemData(event?.active?.data?.current)

  }

  const handleDragEnd = (event) => {
    const {active, over} = event
    if(!over) return

    if(active.id !== over.id) {
      // get old position from active
      const oldIndex = orderdColumns.findIndex(c => c._id === active.id)
      // get new position from over
      const newIndex = orderdColumns.findIndex(c => c._id === over.id)
      
      const dndOrderdColumns = arrayMove(orderdColumns, oldIndex, newIndex)
      
      // const dndOrderdColumnsIds = dndOrderdColumns.map(c => c._id)
      // console.log('dndOrderdColumns: ', dndOrderdColumns)
      // console.log('dndOrderdColumnsIds: ',dndOrderdColumnsIds)
      
      setOrderdColumns(dndOrderdColumns)
    }
    setActiveDragItemData(null)
    setActiveDragItemId(null)
    setActiveDragItemType(null)
  }

  const customDropAnimation =  {
    sideEffects: defaultDropAnimationSideEffects({
      styles: {
        active:{
          opacity: '0.5'
        }
      }
    })
  }

  return (
    <DndContext 
      onDragEnd={handleDragEnd} 
      sensors={sensors}
      onDragStart={handleDragStart} 
      >
      <Box sx={{
          bgcolor: (theme) => (theme.palette.mode === 'dark' ? '#34495e' : '#1976d2' ),
          width: '100%',
          height: (theme) => theme.taskify.boardContentHeight,
          p: '10px 0'
        }}>
          <ListColumns columns={orderdColumns} />
          <DragOverlay dropAnimation={customDropAnimation}>
            {(!activeDragItemType ) && null}
            {(activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.COLUMN) && <Column column={activeDragItemData} />}
            {(activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.CARD) && <Card card={activeDragItemData} />}
          </DragOverlay>
        </Box>
      </DndContext>
  )
}

export default BoardContent
