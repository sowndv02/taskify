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
  defaultDropAnimationSideEffects,
  closestCorners,
  pointerWithin,
  rectIntersection,
  getFirstCollision,
  closestCenter
} from '@dnd-kit/core';
import { useCallback, useEffect, useRef, useState } from 'react';
import { arrayMove } from '@dnd-kit/sortable';
import Column from './ListColumns/Column/Column'
import Card from './ListColumns/Column/ListCards/Card/Card'
import { cloneDeep, isEmpty } from 'lodash'
import { generatePlaceholderCard } from '~/utils/formatters';

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
  const [oldColumnDraggingCard, setOldColumnDraggingCard] = useState(null)

   const lastOverId = useRef(null)



  useEffect(() => {
    setOrderdColumns(mapOrder(board?.columns, board?.columnOrderIds, '_id'))
  }, [board])


  const findColumnByCardId = (cardId) =>{
    return  orderdColumns.find(column => column?.cards?.map(card => card._id)?.includes(cardId))
  }


  const moveCardBetweenDifferentColumns = (
    overColumn,
    overCardId,
    active,
    over,
    activeColumn,
    activeDraggingCardId,
    activeDraggingCardData
  )  =>{
    setOrderdColumns(prevColumns => {
        const overCardIndex = overColumn?.cards?.findIndex(card => card._id === overCardId)
        let newCardIndex
        const isBelowOverItem =
        active.rect.current.translated  &&
        active.rect.current.translated.top > over.rect.top + over.rect.height
        const modifier = isBelowOverItem ? 1 : 0
        newCardIndex = overCardIndex >= 0 ? overCardIndex + modifier: overColumn?.cards?.length + 1
        
        const nextColumns = cloneDeep(prevColumns)
        const nextActiveColumn = nextColumns.find(column => column._id === activeColumn._id)
        const nextOverColumn = nextColumns.find(column => column._id === overColumn._id)

        // old column
        if(nextActiveColumn){
          nextActiveColumn.cards = nextActiveColumn.cards.filter(card => card._id !== activeDraggingCardId)
          if(isEmpty(nextActiveColumn.cards)){
            nextActiveColumn.cards = [generatePlaceholderCard(nextActiveColumn)]
          }
          nextActiveColumn.cardOrderIds = nextActiveColumn.cards.map(card => card._id)
        }

        // new column
        if(nextOverColumn){
          nextOverColumn.cards = nextOverColumn.cards.filter(card => card._id !== activeDraggingCardId)
          
          const rebuildactiveDraggingCardData = {
            ...activeDraggingCardData,
            columnId: nextOverColumn._id
          }
          nextOverColumn.cards = nextOverColumn.cards.toSpliced(newCardIndex, 0, rebuildactiveDraggingCardData)
          
          nextOverColumn.cards = nextOverColumn.cards.filter(card => !card.FE_PlaceholderCard)
          
          nextOverColumn.cardOrderIds = nextOverColumn.cards.map(card => card._id)
        }


        return nextColumns
    })
  }


  // Trigger when start drag component
  const handleDragStart = (event) =>{
    setActiveDragItemId(event?.active?.id)
    setActiveDragItemType(event?.active?.data?.current?.columnId ? ACTIVE_DRAG_ITEM_TYPE.CARD : ACTIVE_DRAG_ITEM_TYPE.COLUMN)
    setActiveDragItemData(event?.active?.data?.current)

    if(event?.active?.data?.current?.columnId) setOldColumnDraggingCard(findColumnByCardId(event?.active?.id))
  }


  // Trigger when drag component
  const handleDragOver = (event) =>{
    if(activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.COLUMN) return

    // handle drag card
    const {active, over} = event 

    if(!active || !over) return

    const {id: activeDraggingCardId, data: { current: activeDraggingCardData }} = active
    const {id: overCardId} = over

    const activeColumn = findColumnByCardId(activeDraggingCardId)
    const overColumn = findColumnByCardId(overCardId)

    if(!activeColumn || !overColumn) return

    if(activeColumn._id !== overColumn._id){
      moveCardBetweenDifferentColumns(
        overColumn,
        overCardId,
        active,
        over,
        activeColumn,
        activeDraggingCardId,
        activeDraggingCardData
      )
    }


  }

  // Trigger when end drag component => drop component
  const handleDragEnd = (event) => {
    const {active, over} = event
    if(!active || !over) return
    
    if(activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.CARD){
      const {id: activeDraggingCardId, data: { current: activeDraggingCardData }} = active
      const {id: overCardId} = over
      const activeColumn = findColumnByCardId(activeDraggingCardId)
      const overColumn = findColumnByCardId(overCardId)
      if(!activeColumn || !overColumn) return

 
      if(oldColumnDraggingCard._id !== overColumn._id){
        moveCardBetweenDifferentColumns(
          overColumn,
          overCardId,
          active,
          over,
          activeColumn,
          activeDraggingCardId,
          activeDraggingCardData
        )
      }else{
         // get old position from oldColumnDraggingCard
        const oldCardIndex = oldColumnDraggingCard?.cards?.findIndex(c => c._id === activeDragItemId)
        // get new position from over
        const newCardIndex = oldColumnDraggingCard?.cards?.findIndex(c => c._id === overCardId)

        const dndOrderdCards = arrayMove(oldColumnDraggingCard?.cards, oldCardIndex, newCardIndex)
        setOrderdColumns(prevColumns => {
          const nextColumns = cloneDeep(prevColumns)

          const targetColumn = nextColumns.find(column => column._id === overColumn._id)
          targetColumn.cards = dndOrderdCards
          targetColumn.cardOrderIds = dndOrderdCards.map(card => card._id)
          
          return nextColumns
        })

      }


    }

    if(activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.COLUMN){
      if(active.id !== over.id) {
        // get old position from active
        const oldColumnIndex = orderdColumns.findIndex(c => c._id === active.id)
        // get new position from over
        const newColumnIndex = orderdColumns.findIndex(c => c._id === over.id)
        
        const dndOrderdColumns = arrayMove(orderdColumns, oldColumnIndex, newColumnIndex)
        
        // const dndOrderdColumnsIds = dndOrderdColumns.map(c => c._id)
        // console.log('dndOrderdColumns: ', dndOrderdColumns)
        // console.log('dndOrderdColumnsIds: ',dndOrderdColumnsIds)
        
        setOrderdColumns(dndOrderdColumns)
      }
    }


    
    setActiveDragItemData(null)
    setActiveDragItemId(null)
    setActiveDragItemType(null)
    setOldColumnDraggingCard(null)
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

  const collisionDetectionStrategy = useCallback((args) =>{
    if(activeDragItemType === ACTIVE_DRAG_ITEM_TYPE.COLUMN){
      return closestCorners({ ...args })
    }

    // 
    const pointerIntersection  = pointerWithin(args)
    if(!pointerIntersection?.length) return

    // const intersections = !!pointerIntersection?.length ? 
    // pointerIntersection : 
    // rectIntersection(args)


    let overId = getFirstCollision(pointerIntersection, 'id') 
    if(overId){

      const checkColumn = orderdColumns.find(column  => column._id === overId)
      if(checkColumn) {
        overId = closestCorners({
          ...args,
          droppableContainers: args.droppableContainers.filter(container => {
            return (container.id !== overId) && (checkColumn?.cardOrderIds?.includes(container.id))
          })
        })[0]?.id
      }

      lastOverId.current = overId 
      return [{id: overId }]
    }

    return lastOverId.current ? [{id: lastOverId.current}] : []

    
  
  }, [activeDragItemType])


  return (
    <DndContext 
      sensors={sensors}
      // collisionDetection={closestCorners}
      collisionDetection={collisionDetectionStrategy}
      onDragStart={handleDragStart}
      onDragOver={handleDragOver} 
      onDragEnd={handleDragEnd} 
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
