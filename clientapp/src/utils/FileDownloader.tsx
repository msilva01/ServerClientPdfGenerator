import React, { useEffect, useState } from "react";
import {
  Button,
  Card,
  CardContent,
  LinearProgress,
  Typography,
} from "@mui/material";
import { Modal } from "react-bootstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faBan, faTimesCircle } from "@fortawesome/free-solid-svg-icons";
import useDownloader from "react-use-downloader";

export interface JuvenileDownloaderProps {
  show?: boolean;
  onHide?: any;
  filename: string;
  name: string;
  url: string;
  title: string;
}

export default function FileDownloader(props: JuvenileDownloaderProps) {
  const [downloading, setDownloading] = useState(false);

  const baseUrl = `${process.env.REACT_APP_BASE_URL}api/`;

  const { size, elapsed, percentage, download, cancel, error, isInProgress } =
    useDownloader({
      mode: "cors",
      headers: {
        Accept: "*/*",
        "Content-Type": "application/octet-stream",
      },
    });

  const fileUrl = `${baseUrl}${props.url}?fileName=${props.filename}&name=${props.name}`;

  useEffect(() => {
    if (downloading && !isInProgress && error == null) {
      props.onHide();
    }
  }, [downloading, isInProgress, error]);

  useEffect(() => {
    if (props.show === true) {
      download(fileUrl, props.name);
      setDownloading(true);
    }
  }, [props.show]);
  return (
    <Modal
      show={props.show}
      size="lg"
      aria-labelledby="contained-modal-title-vcenter"
      centered
      onHide={props.onHide}
    >
      <Modal.Header closeButton className="modal-header primary">
        {props.title}
      </Modal.Header>
      <Modal.Body>
        <Card elevation={0}>
          <CardContent>
            <LinearProgress variant="determinate" value={percentage} />
          </CardContent>
          <CardContent>
            <ul style={{ listStyleType: "none" }}>
              <li>
                <Typography variant="body2" color="textSecondary" component="p">
                  size: <b>{size} bytes</b>
                </Typography>
              </li>
              <li>
                <Typography variant="body2" color="textSecondary" component="p">
                  elapsed: <b>{elapsed}s</b>
                </Typography>
              </li>
              <li>
                <Typography variant="body2" color="textSecondary" component="p">
                  percentage: <b>{percentage}%</b>
                </Typography>
              </li>
              <li>
                {error != null && (
                  <Typography
                    variant="body2"
                    color="textSecondary"
                    component="p"
                  >
                    error: <b>{JSON.stringify(error)}</b>
                  </Typography>
                )}
              </li>
            </ul>
          </CardContent>
        </Card>
      </Modal.Body>
      <Modal.Footer>
        <div>
          <Button
            disabled={!isInProgress}
            color="secondary"
            variant="contained"
            onClick={() => cancel()}
            className="me-2"
          >
            <FontAwesomeIcon icon={faBan}></FontAwesomeIcon>
            &nbsp;Cancel the download
          </Button>
          <Button variant="contained" color="secondary" onClick={props.onHide}>
            <FontAwesomeIcon icon={faTimesCircle}></FontAwesomeIcon>
            &nbsp;Close
          </Button>
        </div>
      </Modal.Footer>
    </Modal>
  );
}
