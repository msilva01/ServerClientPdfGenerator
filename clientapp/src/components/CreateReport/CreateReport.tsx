import { Button, Container } from "@mui/material";
import { useMutation } from "@tanstack/react-query";
import { toast } from "react-toastify";

import Card from "@mui/material/Card";
import CardHeader from "@mui/material/CardHeader";
import CardActions from "@mui/material/CardActions";
import { PostAsync } from "../../utils/DataWorker";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faDownload } from "@fortawesome/free-solid-svg-icons";
import { SignalRContext } from "../../Services/SignalRContext";
import { JobProcessingCallbacksNames, ReportDto } from "../../Services/hub";
import FileDownloader from "../../utils/FileDownloader";
import { useReducer, useState } from "react";

export interface CreateReportProps {
  id?: string;
}

interface DownloadInfo {
  downloadId: string;
  fileName: string;
}

interface DownloadState {
  downloadData: DownloadInfo;
}

interface DownloadAction {
  type: string;
  payload: DownloadInfo;
}

function downloadReducer(state: DownloadState, action: DownloadAction) {
  const { type, payload } = action;
  switch (type) {
    case "add":
      return { downloadData: payload };

    default:
      return state;
  }
}
export function CreateReport({ id = "" }: CreateReportProps) {
  const [state, dispatch] = useReducer(downloadReducer, {
    downloadData: { downloadId: "", fileName: "" },
  });

  const [showDownloadModal, setShowDownloadModal] = useState(false);

  SignalRContext.useSignalREffect(
    JobProcessingCallbacksNames.reportReady,
    (reportDto: ReportDto) => {
      dispatch({
        type: "add",
        payload: { downloadId: reportDto.id, fileName: reportDto.fileName },
      });

      toast.success(`Report Finished ${reportDto.fileName}`);
    },
    []
  );

  SignalRContext.useSignalREffect(
    JobProcessingCallbacksNames.reportError,
    (msg) => {
      toast.error(`An Error occured sending messages to Twilio ${msg}`);
    },
    []
  );

  const { mutate } = useMutation({
    mutationFn: async () =>
      await PostAsync("Report/GenerateReport", { userId: "someId" }).then(
        (r: any) => r.data
      ),
    onSuccess: () => {
      toast.success(
        "The report is being generated. We will notify you when ready."
      );
    },
  });

  return (
    <Container maxWidth="md" className=" p-3 mt-5">
      <FileDownloader
        show={showDownloadModal}
        onHide={() => setShowDownloadModal(false)}
        filename={state?.downloadData?.downloadId}
        name={state?.downloadData?.fileName}
        title="Downloading Report"
        url="Report/Download"
      ></FileDownloader>
      <Card
        elevation={8}
        className="mt-2 p-4"
        style={{ backgroundColor: "#F6F3EE" }}
      >
        <CardHeader title="Report Generator"></CardHeader>
        <CardActions>
          <Button variant="contained" color="success" onClick={() => mutate()}>
            Print Report
          </Button>
          <Button
            variant="contained"
            color="primary"
            disabled={state?.downloadData?.downloadId === ""}
            onClick={() => setShowDownloadModal(true)}
          >
            <FontAwesomeIcon icon={faDownload}></FontAwesomeIcon>&nbsp;Download
          </Button>
        </CardActions>
      </Card>
    </Container>
  );
}
