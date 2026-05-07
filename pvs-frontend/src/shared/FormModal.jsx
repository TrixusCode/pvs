import { Modal, Button } from 'react-bootstrap';

export default function FormModal({
  show,
  title,
  onClose,
  onSubmit,
  loading = false,
  submitLabel = 'Save',
  cancelLabel = 'Cancel',
  children,
  size = 'lg',
  isEditMode = false
}) {
  return (
    <Modal 
      show={show} 
      onHide={onClose}
      size={size}
      centered
      backdrop="static"
      scrollable
    >
      <Modal.Header closeButton>
        <Modal.Title>
          {isEditMode ? `Edit ${title}` : `Create ${title}`}
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {children}
      </Modal.Body>
      <Modal.Footer>
        <Button variant="secondary" onClick={onClose} disabled={loading}>
          {cancelLabel}
        </Button>
        <Button 
          variant="primary" 
          onClick={onSubmit}
          disabled={loading}
        >
          {loading ? 'Saving...' : submitLabel}
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
