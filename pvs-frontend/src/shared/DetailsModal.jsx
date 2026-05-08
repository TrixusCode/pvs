import { Modal, Button } from 'react-bootstrap';

export default function DetailsModal({
  show,
  title,
  onClose,
  children,
  footer,
  size = 'lg'
}) {
  return (
    <Modal show={show} onHide={onClose} size={size} centered scrollable>
      <Modal.Header closeButton>
        <Modal.Title>{title}</Modal.Title>
      </Modal.Header>
      <Modal.Body>{children}</Modal.Body>
      <Modal.Footer>
        {footer}
        <Button variant="secondary" onClick={onClose}>
          Close
        </Button>
      </Modal.Footer>
    </Modal>
  );
}
